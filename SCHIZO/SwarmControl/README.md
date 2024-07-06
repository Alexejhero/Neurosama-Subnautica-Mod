Game client portion of the [Swarm Control](https://github.com/VedalAI/swarm-control) Twitch extension.

Intended for Below Zero, nothing is guaranteed to work in base Subnautica (it's low prio, PRs welcome)

### Setup
After setting up the [server](https://github.com/VedalAI/swarm-control), launch the game (with the mod) and do the following:
- Open the console with `SHIFT + Enter`
- Use the `sc_url` command to set the base URL for the server. e.g. `sc_url https://subnautica.example.com/`
- Set the private API key with `sc_apikey` - e.g. `sc_apikey totallysecretkey`
- Go into `Options` -> `Mods` and click `Connect Swarm Control`
- After the game connects and you load a save, any redeems you have configured should become available!

Running `sc_export` in the console will save the config json (without the `version` field) to your clipboard.
