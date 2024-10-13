using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace TMGenerator;

public class Settings
{
    [MaintainOrder]
    
    [SettingName("FeatureTMFriendlierFactions")]
    [Tooltip("I'm tired of always running into dead enemies.")]
    public bool FeatureTMFriendlierFactions = false;
    
    [SettingName("FeatureReduceAggressionForFCOM")]
    [Tooltip("I'm tired of always running into dead enemies.")]
    public bool FeatureReduceAggressionForFCOM = true;
}