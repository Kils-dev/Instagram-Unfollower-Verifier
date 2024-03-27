using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Instagram_Unfollower_verifier
{
    public partial class MainWindow : Window
    {
        private bool exceptionListChanged = false;
        public MainWindow()
        {
            InitializeComponent();
            OUTPUTCONSOLE.Text = "====================================================" + "\r\n" + "Welcome to Instagram Unfollower Verifier! Click the 'ⓘ' icon for see the latest changes." + "\r\n" + "====================================================" + "\r\n";
            TXT_CUSTOMAMOUNT.PreviewTextInput += TextBox_PreviewTextInput;
            TXT_CUSTOMAMOUNT.TextChanged += TextBox_TextChanged;
            EXEPTIONLIST.TextChanged += ExceptionList_TextChanged;
            BTN_SAVELIST.IsEnabled = false;
        }

        private void ExceptionList_TextChanged(object sender, TextChangedEventArgs e)
        {
            exceptionListChanged = true;
            BTN_SAVELIST.IsEnabled = !string.IsNullOrEmpty(EXEPTIONLIST.Text);
        }

        private void BTN_BROWSEFOLDER_Click(object sender, RoutedEventArgs e)
        {
            BTN_COMPARE.IsEnabled = false;
            BTN_OPENALL.IsEnabled = false;
            BTN_OPENCUSTOM.IsEnabled = false;
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                string folderPath = dialog.SelectedPath;
                TXT_FOLDERSOURCE.Text = folderPath;
                string filePath1 = System.IO.Path.Combine(folderPath, "following.json");
                string filePath2 = System.IO.Path.Combine(folderPath, "followers_1.json");

                if (!File.Exists(filePath1) && !File.Exists(filePath2))
                {
                    OUTPUTCONSOLE.Text += "\r\n" + "[⚠WARNING]: Missing the required files inside the selected folder. Please select the correct folder containing:" + "\r\n" + "- following.json" + "\r\n" + "- followers_1.json" + "\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                }
                else
                {
                    OUTPUTCONSOLE.Text += "\r\n" + "[🛈INFO]: Found the required files following.json and followers_1.json" + "\r\n" + "You can now compare your followers/unfollowers" + "\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                    BTN_COMPARE.IsEnabled = true;
                }
            }
        }

        private void BTN_CLEARCONSOLE_Click(object sender, RoutedEventArgs e)
        {
            OUTPUTCONSOLE.Clear();
        }

        private void BTN_COMPARE_Click(object sender, RoutedEventArgs e)
        {
            BTN_OPENALL.IsEnabled = true;
            BTN_OPENCUSTOM.IsEnabled = true;
            string followersJsonPath = Path.Combine(TXT_FOLDERSOURCE.Text, "followers_1.json");
            string followingJsonPath = Path.Combine(TXT_FOLDERSOURCE.Text, "following.json");

            try
            {
                string followersJson = File.ReadAllText(followersJsonPath);
                string followingJson = File.ReadAllText(followingJsonPath);

                CompareJsonValues(followersJson, followingJson);
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text = $"[⛔ERROR]: {ex.Message}";
            }
        }

        private void CompareJsonValues(string followersJson, string followingJson)
        {
            try
            {
                var regex = new Regex("\"value\":\\s*\"([^\"]+)\"");

                // Extract values from both JSON strings
                var followersValues = regex.Matches(followersJson).Select(match => match.Groups[1].Value).ToHashSet();
                var followingValues = regex.Matches(followingJson).Select(match => match.Groups[1].Value).ToHashSet();

                // Get the exception list from the textbox
                var exceptionList = EXEPTIONLIST.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToHashSet();

                // Find values that exist in followingValues but not in followersValues
                var unfollowers = followingValues.Except(followersValues);

                // Convert to HashSet<string> before using ExceptWith
                var excludedExceptions = new HashSet<string>(unfollowers.Intersect(exceptionList));

                // Remove excluded exceptions from the Unfollowers list
                unfollowers = unfollowers.Except(excludedExceptions);

                // Display differences in the output console
                OUTPUTCONSOLE.Text = "Unfollowers:" + "\r\n" + "------------------" + Environment.NewLine + string.Join(Environment.NewLine, unfollowers) +
                                     Environment.NewLine + Environment.NewLine + "Excluded Exception list users:" + "\r\n" + "------------------" + Environment.NewLine +
                                     string.Join(Environment.NewLine, excludedExceptions);
                OUTPUTCONSOLE.ScrollToEnd();
                SaveDifferencesToFile(unfollowers, excludedExceptions);
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text = $"[⛔ERROR]: {ex.Message}";
            }
        }

        private string GenerateFilePath()
        {
            string currentDate = DateTime.Now.ToString("dd-MM-yyyy");
            string fileName = $"UNFOLLOWERS-{currentDate}.txt";
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
        }


        private void SaveDifferencesToFile(IEnumerable<string> unfollowers, IEnumerable<string> excludedExceptions)
        {
            try
            {
                string filePath = GenerateFilePath();

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("Unfollowers:");
                    foreach (var unfollower in unfollowers)
                    {
                        writer.WriteLine(unfollower);
                    }

                    writer.WriteLine("Excluded Exception list users:");
                    foreach (var exception in excludedExceptions)
                    {
                        writer.WriteLine(exception);
                    }
                }

                OUTPUTCONSOLE.Text += $"\r\n\r\n[☑SUCCESS]: Unfollowers saved to: {filePath}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Saving Unfollowers to file: {ex.Message}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
        }

        private void BTN_OPENALL_Click(object sender, RoutedEventArgs e)
        {
            string filePath = GenerateFilePath();

            try
            {
                if (File.Exists(filePath))
                {
                    string[] unfollowersLines = File.ReadAllLines(filePath);
                    List<string> unfollowerUsernames = new List<string>();

                    bool isUnfollowersSection = false;

                    // Iterate through the lines and extract Unfollower usernames
                    foreach (string line in unfollowersLines)
                    {
                        if (line.StartsWith("Unfollowers:"))
                        {
                            isUnfollowersSection = true;
                            continue;
                        }

                        if (isUnfollowersSection && line.StartsWith("Excluded Exception list users:"))
                        {
                            break;
                        }

                        if (isUnfollowersSection && !string.IsNullOrWhiteSpace(line))
                        {
                            unfollowerUsernames.Add(line.Trim());
                        }
                    }

                    // Open Instagram URLs for each unfollower
                    foreach (string username in unfollowerUsernames)
                    {
                        string instagramUrl = $"https://www.instagram.com/{username}";

                        try
                        {
                            OUTPUTCONSOLE.Text += $"\r\n[🛈INFO]: Opening Instagram URL for {username}: {instagramUrl}\r\n";
                            OUTPUTCONSOLE.ScrollToEnd();

                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                            {
                                FileName = "cmd",
                                Arguments = $"/c start {instagramUrl}",
                                UseShellExecute = false,
                                RedirectStandardOutput = true,
                                CreateNoWindow = true
                            });
                            System.Threading.Thread.Sleep(1000);
                        }
                        catch (Exception ex)
                        {
                            OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Opening Instagram URL for {username}: {ex.Message}\r\n";
                            OUTPUTCONSOLE.ScrollToEnd();
                        }
                    }

                    OUTPUTCONSOLE.Text += "\r\n\r\n====================================================" + $"\r\n[☑SUCCESS]: Opened all Instagram URLs for unfollowers.\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                }
                else
                {
                    OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Unfollower list file does not exist.\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Reading file or opening Instagram URLs: {ex.Message}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
        }

        private List<string> unfollowerUsernames = new List<string>();
        private HashSet<string> openedProfiles = new HashSet<string>();

        private int currentIndex = 0;

        private void BTN_OPENCUSTOM_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = GenerateFilePath();

                if (File.Exists(filePath))
                {
                    string[] unfollowersLines = File.ReadAllLines(filePath);

                    bool isUnfollowersSection = false;
                    List<string> unfollowerUsernames = new List<string>();

                    // Populate the list with usernames
                    foreach (string line in unfollowersLines)
                    {
                        if (line.StartsWith("Unfollowers:"))
                        {
                            isUnfollowersSection = true;
                            continue;
                        }

                        if (isUnfollowersSection && line.StartsWith("Excluded Exception list users:"))
                        {
                            break;
                        }

                        if (isUnfollowersSection && !string.IsNullOrWhiteSpace(line))
                        {
                            string username = line.Trim();
                            unfollowerUsernames.Add(username);
                        }
                    }

                    if (int.TryParse(TXT_CUSTOMAMOUNT?.Text, out int customAmount))
                    {
                        HashSet<string> openedProfiles = new HashSet<string>();

                        for (int i = 0; i < customAmount; i++)
                        {
                            if (currentIndex < unfollowerUsernames.Count)
                            {
                                string username = unfollowerUsernames[currentIndex];

                                if (!openedProfiles.Contains(username))
                                {
                                    OpenInstagramUrl(username);
                                    openedProfiles.Add(username);
                                }

                                currentIndex++;
                            }
                            else
                            {
                                OUTPUTCONSOLE.Text += "\r\n[🛈INFO]: No more unfollower profiles to open.\r\n";
                                currentIndex = 0;
                                OUTPUTCONSOLE.ScrollToEnd();
                                break;
                            }
                        }

                        OUTPUTCONSOLE.Text += $"\r\n\r\n[☑SUCCESS]: Opened {customAmount} Instagram URLs.\r\n";
                        OUTPUTCONSOLE.ScrollToEnd();
                    }
                    else
                    {
                        OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Invalid custom amount.\r\n";
                        OUTPUTCONSOLE.ScrollToEnd();
                    }
                }
                else
                {
                    OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Unfollower list file does not exist.\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Reading file or opening Instagram URLs: {ex.Message}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
        }



        private void OpenInstagramUrl(string username)
        {
            string instagramUrl = $"https://www.instagram.com/{username}";

            try
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[🛈INFO]: Opening Instagram URL for {username}: {instagramUrl}\r\n";

                // Open the Instagram URL in the default web browser
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = instagramUrl,
                    UseShellExecute = true
                });

                Thread.Sleep(2300);
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Opening Instagram URL for {username}: {ex.Message}\r\n";
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(TXT_CUSTOMAMOUNT.Text, out int enteredValue) && enteredValue > 30)
            {
                MessageBox.Show("The maximum amount is 30 (which is already a high number).", "Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                TXT_CUSTOMAMOUNT.Text = "30";
            }
        }

        private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            if (!IsNumeric(e.Text))
            {
                e.Handled = true;
            }
        }

        private bool IsNumeric(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]+$");
        }

        private void BTN_SAVELIST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (exceptionListChanged)
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NameList.txt");

                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }

                    // Save the new exception list
                    File.WriteAllText(filePath, EXEPTIONLIST.Text);

                    OUTPUTCONSOLE.Text += $"\r\n\r\n[☑SUCCESS]: Exception list saved to: {filePath}\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                    exceptionListChanged = false;
                    BTN_SAVELIST.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Saving Exception list to file: {ex.Message}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
        }

        private void BTN_LOADLIST_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "NameList.txt");

                if (File.Exists(filePath))
                {
                    // Load the exception list from the file
                    string exceptionList = File.ReadAllText(filePath);
                    EXEPTIONLIST.Text = exceptionList;

                    OUTPUTCONSOLE.Text += $"\r\n\r\n[☑SUCCESS]: Exception list loaded from: {filePath}\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                    exceptionListChanged = false;
                    BTN_SAVELIST.IsEnabled = false;
                }
                else
                {
                    OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Exception list file does not exist. Please create one first.\r\n";
                    OUTPUTCONSOLE.ScrollToEnd();
                }
            }
            catch (Exception ex)
            {
                OUTPUTCONSOLE.Text += $"\r\n\r\n[⛔ERROR]: Loading Exception list from file: {ex.Message}\r\n";
                OUTPUTCONSOLE.ScrollToEnd();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/Kils-dev/Instagram-Unfollower-Verifier/tree/main";
            System.Diagnostics.Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }

    }
}