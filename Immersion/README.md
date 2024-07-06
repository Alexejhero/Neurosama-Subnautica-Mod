Basic cruddy "send stuff to LLM" mod.

### Setup

Set up a web server at your chosen URL that will accept POST requests on the following endpoints:
- `/mute` - mute/unmute (to reduce blabbering during cutscenes)
    - accepts `True` and `False` in POST body
- `/priority` and `/non-priority` - send messages (general context/react stuff)
    - accepts the message as a raw string in POST body (e.g. `Something happened`, note no quotation marks)

Then, go in game and run the following commands in the console (`SHIFT + Enter` to open):
- `immersion set player Your Name`
- `immersion set url http://yoururl:12345`
    - If your API uses auth, run `immersion set apikey yoursecretapikey`
- `immersion set pronouns you/your`

The default configuration is [defined here](./Globals.cs#L11).

### Console commands
Available console commands are:
- `immersion set <option> <value>` - set config value
    - Available options are `player`, `url`, `apikey`, and `pronouns`
    - For `pronouns`, there are several [predefined sets of pronouns](./Formatting/PronounSet.cs#L27)
    - Otherwise, the command output will guide you to the correct formatting
- `immersion trackers` - list available trackers
- `immersion <enable|disable> [tracker...]` - toggle one or more trackers
    - If no trackers are specified (i.e. `immersion enable`) all trackers will be affected
    - For a list of available trackers, run `immersion trackers`
- `immersion <prio|noprio>` - force low prio
    - While active, all high-priority messages will be routed to the `non-priority` endpoint.
    - `noprio` activates this effect and `prio` deactivates it.
- `immersion <react|send> <message>` - manually send with high (for `react`) or low (for `send`) priority.
    - Affected by `noprio`.
- `immersion <mute|unmute>` - manual mute/unmute.
