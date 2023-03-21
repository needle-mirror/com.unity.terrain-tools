#if UNITY_2018_2_OR_NEWER
#define NEW_PACKMAN

using System;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.UI;
using UnityEngine;

#if UNITY_2019_1_OR_NEWER
using UnityEngine.UIElements;
#else
using UnityEngine.Experimental.UIElements;
#endif

[UnityEditor.InitializeOnLoad]
internal class SamplesLinkPackageManagerExtension : IPackageManagerExtension
{
    VisualElement rootVisualElement;
    const string SAMPLEBUTTON_TEXT = "Download Asset Samples from Asset Store";
    private const string SAMPLESCENEURPBUTTON_TEXT = "Download URP Demo Scene from Asset Store";
    private const string SAMPLESCENEHDRPBUTTON_TEXT = "Download HDRP Demo Scene from Asset Store";
    const string ASSETSTORE_URL = "http://u3d.as/1wLg";
    private const string URPSCENE_URL = "https://u3d.as/2L6J";
    private const string HDRPSCENE_URL = "https://u3d.as/2L6K";
    const string TERRAIN_TOOLS_NAME = "com.unity.terrain-tools";

    private Button samplesButton;
    private Button sampleSceneButton;
    private VisualElement parent;

    public VisualElement CreateExtensionUI()
    {
        samplesButton = new Button();
        samplesButton.text = SAMPLEBUTTON_TEXT;
        samplesButton.clickable.clicked += () => Application.OpenURL(ASSETSTORE_URL);

        CreateDemoSceneButton();

        return samplesButton;
    }

    void CreateDemoSceneButton()
    {
        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset == null)
        {
            return;
        }

        sampleSceneButton = new Button();

        if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().FullName
                 == "UnityEngine.Rendering.HighDefinition.HDRenderPipelineAsset")
        {
            sampleSceneButton.text = SAMPLESCENEHDRPBUTTON_TEXT;
            sampleSceneButton.clickable.clicked += () => Application.OpenURL(HDRPSCENE_URL);
        }
        else if (UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType().FullName
                 == "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset")
        {
            sampleSceneButton.text = SAMPLESCENEURPBUTTON_TEXT;
            sampleSceneButton.clickable.clicked += () => Application.OpenURL(URPSCENE_URL);
        }        
    }

    static SamplesLinkPackageManagerExtension()
    {
        PackageManagerExtensions.RegisterExtension(new SamplesLinkPackageManagerExtension());
    }

    void IPackageManagerExtension.OnPackageSelectionChange(PackageInfo packageInfo)
    {
        // Prevent the button from rendering on other packages
        if (samplesButton.parent != null)
            parent = samplesButton.parent;

        bool shouldRender = packageInfo?.name == TERRAIN_TOOLS_NAME;
        if (!shouldRender)
        {
            samplesButton.RemoveFromHierarchy();

            if (sampleSceneButton != null)
            {
                sampleSceneButton.RemoveFromHierarchy();
            }
        }
        else
        {
            parent.Add(samplesButton);

            if (sampleSceneButton != null)
            {
                parent.Add(sampleSceneButton);
            }
        }
    }

    void IPackageManagerExtension.OnPackageAddedOrUpdated(PackageInfo packageInfo) { }

    void IPackageManagerExtension.OnPackageRemoved(PackageInfo packageInfo) { }
}

#endif