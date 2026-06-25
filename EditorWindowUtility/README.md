## EditorWindowUtility

### Concept

Close focused windows and reopen recently closed ones browser-like.

### Features

- Closed windows stack
- You can assign any hotkey in Edit > Shortcuts (under the Window/ category)

### Usage

- Shortcut: `Ctrl + W` (Close focused window)
- Shortcut: `Ctrl + Shift + T` (Reopen closed window)

### Limitations

- Windows closed outside the tool (e.g. using the MMB or Close Tab option) cannot be reopened because they are not being registered in the stack
- Does not restore the docking state of closed windows when reopened