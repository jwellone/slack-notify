[![GitHub license](https://img.shields.io/github/license/jwellone/slack-notify.svg?style=plastic)](https://github.com/jwellone/slack-notify/blob/main/LICENSE)
[![GitHub Release](https://img.shields.io/github/v/release/jwellone/slack-notify.svg?style=plastic)](https://GitHub.com/jwellone/slack-notify/releases/latest)
[![Github All Releases](https://img.shields.io/github/downloads/jwellone/slack-notify/total?color=blue&style=plastic)](https://GitHub.com/jwellone/slack-notify/releases)
![GitHub repo size](https://img.shields.io/github/repo-size/jwellone/slack-notify?label=size&style=plastic)
![C#](https://img.shields.io/badge/C%23-239120?logo=c-sharp&style=plastic)
[![Unity](https://img.shields.io/badge/Unity-100000?logo=unity&style=plastic)](https://unity.com)


# slack-notify
Can use the slack api to send information to a specified channel.
- Simple message.
- Screenshot.
- Log information.

This feature is one of the "j1Tech" developed by jwellone Inc for Unity.


## Getting Started
- Prepare a slack account and an [apps](https://api.slack.com/apps).
- Import the package.
- Open SampleScene.unity.
- Select the SampleScene object on the Hierarchy and enter the Token and Channel information in the Inspector.
- After running the editor, entering the title and message, press any submit button to send the information to slack.
- [Here is a sample program.](https://github.com/jwellone/slack-notify/blob/main/Assets/jwellone/Slack/Sample/Scritps/SlackSampleScene.cs) 

![slack-notify](https://user-images.githubusercontent.com/85072161/132600052-266d6192-db8b-42d8-9515-441b6f480da4.png)
It is also possible to create your own class by inheriting the Slack.IProvider class and ISlackAPI class.

If you enter "ENABLE_SLACK_API_LOG" in Scripting Define Symbols, the communication log will be output.

## Authors
Developer:[Yasuhiko Usui.](https://github.com/UsuiYasuhiko-jw1)


[![GitHub watchers](https://img.shields.io/github/watchers/jwellone/slack-notify.svg?style=social&label=Watch)](https://GitHub.com/jwellone/slack-notify/watchers/)
[![GitHub stars](https://img.shields.io/github/stars/jwellone/slack-notify.svg?style=social&label=Stars)](https://GitHub.com/jwellone/slack-notify/stargazers)
[![GitHub fork](https://img.shields.io/github/forks/jwellone/slack-notify.svg?style=social&label=Fork)](https://GitHub.com/jwellone/slack-notify/network/members)
[![Twitter](https://img.shields.io/twitter/follow/jwellone?label=Twitter&logo=twitter&style=social)](http://twitter.com/jwellone)
[![Facebook](https://img.shields.io/badge/Facebook-1877F2?style=for-the-badge&logo=facebook&logoColor=white&style=plastic)](https://www.facebook.com/jwellone)
