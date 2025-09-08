# Typescript Setup

## Install Deno
via npm with `npm install -g deno`.
or via brew with `brew install deno`.
or find the full list of options (here)[https://docs.deno.com/runtime/getting_started/installation/]

## Run
Run locally with `deno run dev`
(will also install dependencies)

The server should now be running locally on port 3000, and you can execute requests using the `./rest/chat.rest` file.
(VS Code REST Client Plugin)[https://marketplace.visualstudio.com/items?itemName=humao.rest-client]

## IDE Setup (optional)
Your IDE may understand Deno better by adding a corresponding plugin:
- VS Code: https://marketplace.visualstudio.com/items?itemName=denoland.vscode-deno
- Jetbrains: https://plugins.jetbrains.com/plugin/14382-deno