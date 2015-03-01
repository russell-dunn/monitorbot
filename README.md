Chatbot, currently running against slack on [redgate-compare.slack.com](https://redgate-compare.slack.com/).

### Building & running

- Should build after a nuget package restore 
- May need ASP.NET MVC to be installed for the System.Web.Helpers dll - use the Web Platform Installer and search for MVC
- Tests are split up into `fasttests` (unit, isolated) and `slowtests` (integration)
- To run, find `scbot/bin/Debug/scbot.exe.config` and fill in the details, then run `scbot.exe`. 

### Features 

- Prints html `<title>`s for links pasted in chat. Has special handlers for jira/zendesk links
- Handles commands in chat (`scbot: do something`) or in PM (`do something`)
- Simple note tracking (per-user): (`scbot: note this feature` / `notes` / `delete note 1`)
- Zendesk issue tracking (per-channel): (`scbot: track ZD#12345` / [time passes] / `1 comment(s) were added to ZD#12345`)

experimental / half-done:
- #2: Teamcity tcWebhooks integration (per-channel): (`scbot: track breakages on branch master`)
- #14: Automated code review: (`scbot: review c78d33a1864`)

### Design

Tries to be functional (pure, immutable) where possible - but since most relevant features call out to third-party services a lot, this isn't always possible.

Has partially ended up being an experiment in SRP - most interfaces have two or three implementations that get chained together to provide different aspects.
It seems to be leaning towards a plugin system as well.
