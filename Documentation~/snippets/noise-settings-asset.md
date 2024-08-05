Use the **Noise Settings Asset** field to save and load curated Noise Settings assets. Note that there are two possible states:

1. When there is no Noise Settings asset assigned, Unity uses an internal instance of a Noise Settings asset, which you can still modify through the UI. Two buttons appear: **Reset** and **Save As**.
2. When there is a Noise Settings asset assigned, Unity copies the settings stored in that asset. When you modify the noise settings through the UI, Unity only applies those changes to the copy of the Noise Settings asset, not the original Noise Settings asset that is stored on disk. This prevents you from overwriting the original with unwanted changes as you tweak the values. Three buttons appear: **Revert**, **Apply**, and **Save As**.

Below are descriptions for each of the buttons that might accompany the **Noise Settings Asset** field.

| **Property** | **Description**                                              |
| ------------ | ------------------------------------------------------------ |
| **Reset**    | Resets the Noise Settings Asset to the default built-in settings. |
| **Revert**   | Reverts the settings of the active Noise Settings Asset to the settings of the original reference that is stored on disk. |
| **Apply**    | Writes the current settings to the assigned reference stored on disk. |
| **Save As**  | Opens a dialog box that allows you to save the settings to a new Noise Settings Asset in your project’s `Assets` folder. Also assigns the newly-created Noise Settings Asset in the field. |