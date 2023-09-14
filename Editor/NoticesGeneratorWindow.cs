using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class NoticesGeneratorWindow : EditorWindow
    {
        private TextField _fileNameField;
        private TextField _headerField;

        private Toggle _offlineModeToggle;
        private Toggle _includeIndirectDependenciesToggle;
        private Toggle _excludeBuiltInPackagesToggle;
        private Toggle _writeToStreamingAssetsFolderToggle;

        private Button _generateButton;

        [MenuItem("Window/Third Party Notices Generator")]
        public static void ShowExample()
        {
            var window = GetWindow<NoticesGeneratorWindow>();
            window.titleContent = new GUIContent("Third Party Notices Generator");
        }

        public void CreateGUI()
        {
            var root = rootVisualElement;

            _fileNameField = new TextField("Notices file name")
            {
                value = NoticesGenerator.DefaultFileName
            };
            root.Add(_fileNameField);

            _headerField = new TextField("File header", -1, true, false, '*')
            {
                value = NoticesGenerator.DefaultHeader
            };
            root.Add(_headerField);

            _offlineModeToggle = new Toggle("Offline mode")
            {
                value = false,
                tooltip =
                    "Specifies whether or not the Package Manager requests the latest information about the project's packages from the remote Unity package registry. When offlineMode is true, the PackageManager.PackageInfo objects in the PackageCollection returned by the Package Manager contain information obtained from the local package cache, which could be out of date."
            };
            root.Add(_offlineModeToggle);

            _includeIndirectDependenciesToggle = new Toggle("Include indirect dependencies")
            {
                value = true,
                tooltip =
                    "Set to true to include indirect dependencies in the PackageCollection returned by the Package Manager. Indirect dependencies include packages referenced in the manifests of project packages or in the manifests of other indirect dependencies. Set to false to include only the packages listed directly in the project manifest."
            };
            root.Add(_includeIndirectDependenciesToggle);

            _excludeBuiltInPackagesToggle = new Toggle("Exclude built-in packages")
            {
                value = true,
                tooltip = "Skip built-in Unity packages."
            };
            root.Add(_excludeBuiltInPackagesToggle);

            _writeToStreamingAssetsFolderToggle = new Toggle("Write file to the StreamingAssets folder")
            {
                value = true,
                tooltip = "Write the file to Streaming Assets folder."
            };
            root.Add(_writeToStreamingAssetsFolderToggle);

            _generateButton = new Button(OnGenerateButtonHandler)
            {
                text = "Generate"
            };
            root.Add(_generateButton);
        }

        private async void OnGenerateButtonHandler()
        {
            _generateButton.SetEnabled(false);
            var initialButtonText = _generateButton.text;
            _generateButton.text = "Fetching packages list...";

            await NoticesGenerator.Generate(_offlineModeToggle.value, _includeIndirectDependenciesToggle.value,
                _excludeBuiltInPackagesToggle.value, _writeToStreamingAssetsFolderToggle.value, _fileNameField.value,
                _headerField.value);

            _generateButton.text = initialButtonText;
            _generateButton.SetEnabled(true);
        }
    }
}
