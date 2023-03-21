The **Noise Field Preview** is an interactive preview that shows the output of the current noise and fractal variant that you set using the Noise Settings.

Depending on the noise and fractal variant, you might see different colors in the Noise Field Preview. Here are the different colors and their meanings.

| **Color**     | **Description** |
| ------------- | ----------------------------------------------------------- |
| **Grayscale** | Output values are within the 0-1 range, where black is 0 and white is 1. |
| **Cyan**      | Output values are negative. |
| **Black**     | Output values are 0. |
| **White**     | Output values are 1. |
| **Red**       | Output values are above 1. This helps you debug any normalization issues when exporting noise Textures. It's best to have exported values that are unsigned and normalized between the 0-1 range. |

**Controls**

- **Translation and Panning**: Click on the preview to pan the noise field, and drag the cursor to modify the translation of the noise field.
- **Scale and Zoom**: To modify the scale of the noise field, place the cursor on the preview, and scroll to zoom in or out.