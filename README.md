# Third Party Notices Generator Utility

This a tool designed for generating third-party notices for your Unity project. It helps you create a list of third-party packages used in your project, making it easier to comply with licensing requirements and provide proper attribution.

## Features

- **Notices file name:** Specify the name of the notices file you want to generate.
- **File header:** Customize the header for the notices file, allowing you to include relevant information.
- **Offline mode:** Enable this option to generate notices using cached package information, useful when you're working offline.
- **Include indirect dependencies:** Choose whether to include indirect package dependencies in the notices.
- **Exclude built-in packages:** Skip built-in Unity packages from the generated notices.
- **Write file to the StreamingAssets folder:** Save the generated notices file to the StreamingAssets folder within your Unity project.

## How to Use

1. Open the Unity Editor.
2. Navigate to `Window > Third Party Notices Generator` to open the utility window.
3. Customize the settings as needed.
4. Click the "Generate" button to create the third-party notices file.

## Important Notes

- The utility fetches package information from the Unity package registry.
- Ensure that you have an active internet connection when using this tool, especially if you want to fetch the latest package information.

Feel free to use this utility to simplify the process of managing third-party notices for your Unity project. It can help you stay compliant with licensing requirements and provide proper attribution for the packages you use.
