using System;

[Serializable]
public class ChildPanelData
{
    public string childName;
    public string text;
    public string imagePath;
}

[Serializable]
public class ParentPanelData
{
    public ChildPanelData[] SexualPanel;
    public ChildPanelData[] AsexualPanel;
}
