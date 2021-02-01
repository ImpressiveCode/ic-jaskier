# Jaskier - extension for VS Code
Defect prediction extension for VS Code. Based on machine learning models created with continuous integration build results as feature it will try to predict if the changes you are currently making in your project will likely result in build failure or not.

## Requirements

Requires [git](http://git-scm.com/downloads) to be installed and that it can be called using the command `git`.

## Installation from package (VSIX)

1. Launch VS Code.
2. Open Command Palette (`Ctrl+Shift+P`).
3. Run `Extensions: Install from VSIX...`.
4. Select the .vsix file and click install.

More details: https://code.visualstudio.com/docs/editor/extension-gallery#_install-from-a-vsix

## Configuration

Configuration can be changed in the VS Code's settings (`Ctrl+Comma`).

- `jaskier.serviceUrl`
    - Url to the service used by this extension.
- `jaskier.debugMode`
    - If true extension writes additional information to the output.

## How it works

- Extension is activated whenever a folder is opened.
- Extension pulls predictions:
    - on activation of this extension,
    - on saving the changes in files,
    - when user executes `Refresh predictions` command. 
        - Press `Ctrl+Shift+P` to open the Command Palette.

## Predictions presentation

- Latest state of predictions is shown in the status bar.
- Details of predictions are presented in the output channel.
    - Click on the Jaskier's status bar item to open the output channel.

## Code metrics (cyclomatic complexity)

- For better predictions the plugin calculates the cyclomatic complexity of changed files.
- This plugin supports the calculation of the cyclomatic complexity for TypeScript (.ts) and JavaScript (.js) files.

## Known issues

- If the path of the GIT repository's main folder is different than the path you have open in the VSCode, the following issues occur:
    - Cyclomatic complexity is not calculated.
    - The file paths printed in the output are incorrect.