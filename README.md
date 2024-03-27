# Instagram Unfollower Verifier

> [!IMPORTANT]
> - Make sure to READ everything below carefully before using the application.
> - This is a bit more reliable than logging-in on those "**Instagram Follower**" apps you get in the **Appstore/IOS store**. Also avoids you getting hacked as a bonus.
> - Just a heads-up, while using the automated way of opening the unfollowers URLs **Instagram** has some funky rules in place, including temporary rate limits on actions like opening Instagram URLs and searching for them. This usually clears up after a few hours.
> - So I would recommend avoiding the "**Open unfollowers**" functionality from the app and just copying the names manually from the app console and pasting each one inside of your **profile -> following -> (Insert the name in the search bar) -> manually click unfollow.** in order to avoid the rate limit functionality on Instagram.
> - Your anti-virus might block the exe, to avoid this if you have Visual Studio just build the application your self using the provided files in the rep.
> - If you have a better implementation/optimizations to the current solution please send a PR, thanks :)

## How to use this aplication:
1. Follow the guide bellow to download the required file:
   - https://help.instagram.com/181231772500920?cms_id=181231772500920
> [!WARNING]
> - Make sure the download format is <b>JSON</b> and not <b>HTML</b>.
> - You will only need to select followers and unfollowers, nothing else.

2. Extract the folder anywhere you like.
3. Open the <b>"Instagram Unfollower Verifier"</b> application.
4. Using the <b>browse button</b>, select the folder path where the JSON files are visible.
5. After the console shows "**[🛈INFO]: Found the required files following.json and followers_1.json**" you may proceed to step 6.
6. Click the "**Compare Follow/Unfollow button**". This will save a file with the current date the action was executed.
> [!TIP]
> - You can setup an Exeption List if you want using the provided "**User Exeption List**" text field.
> - You may also save this list for later use.
> - In case you made any changes to the "**User Exeption List**", either by saving or loading your list, make sure to click the "**Compare Follow/Unfollow button**" again.
7. Either use the "**Open unfollowers (all)**" (**_NOT RECOMMENDED BY ME, SPECIALLY FOR BIG LISTS_**) or "**Open unfollowers (custom)**".
8. After that, you can manually click the unfollow button when the Instagram page opens in your browser.

## DISCLAIMER:
> [!CAUTION]
> Before delving into the realm of unfollowers/followers with my application, I feel it's crucial to clarify my stance regarding Instagram's terms of service.
While my application is designed to enhance your Instagram experience by providing insightful analytics regarding your followers/unfollowers, it's imperative to acknowledge that Instagram has established stringent guidelines governing user conduct. As such, any actions taken on Instagram, including those facilitated through our application, must adhere to these terms.<br><br>
I wish to emphasize that me, as the developers of this application, cannot assume responsibility for any breaches of Instagram's terms of service, including but not limited to account termination or bans. My role is to provide a tool for analysis, not to dictate or control user behavior.<br><br>
I urge all users to familiarize themselves with Instagram's terms of service and community guidelines to ensure compliance and mitigate any potential risks associated with their Instagram activity.
Thank you for your understanding and cooperation.

## Resources:
* [Ookii.Dialogs.Wpf](https://github.com/ookii-dialogs/ookii-dialogs-wpf)
