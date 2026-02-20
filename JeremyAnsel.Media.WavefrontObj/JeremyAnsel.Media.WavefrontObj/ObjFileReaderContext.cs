// <copyright file="ObjFileReaderContext.cs" company="Jérémy Ansel">
// Copyright (c) 2017, 2019 Jérémy Ansel
// </copyright>
// <license>
// Licensed under the MIT license. See LICENSE.txt
// </license>

namespace JeremyAnsel.Media.WavefrontObj;

internal class ObjFileReaderContext
{
    private readonly ObjFile _obj;

    public ObjFileReaderContext(ObjFile obj, ObjFileReaderSettings settings)
    {
        _obj = obj;
        Settings = settings;

        GroupNames = new List<string>();
    }

    public ObjFileReaderSettings Settings { get; }

    public string? ObjectName { get; set; }

    public int LevelOfDetail { get; set; }

    public string? MapName { get; set; }

    public string? MaterialName { get; set; }

    public long SmoothingGroupNumber { get; set; }

    public bool IsBevelInterpolationEnabled { get; set; }

    public bool IsColorInterpolationEnabled { get; set; }

    public bool IsDissolveInterpolationEnabled { get; set; }

    public int MergingGroupNumber { get; set; }

    public ObjFreeFormType FreeFormType { get; set; }

    public bool IsRationalForm { get; set; }

    public int DegreeU { get; set; }

    public int DegreeV { get; set; }

    public float[]? BasicMatrixU { get; set; }

    public float[]? BasicMatrixV { get; set; }

    public float StepU { get; set; }

    public float StepV { get; set; }

    public ObjApproximationTechnique? CurveApproximationTechnique { get; set; }

    public ObjApproximationTechnique? SurfaceApproximationTechnique { get; set; }

    public ObjFreeFormElement? CurrentFreeFormElement { get; set; }

    public List<string> GroupNames { get; private set; }

    private List<ObjGroup>? _currentGroups;

    public List<ObjGroup> GetCurrentGroups()
    {
        if (_currentGroups is not null) return _currentGroups;
        _currentGroups = [_obj.DefaultGroup];
        return _currentGroups;
    }

    public void CreateGroups()
    {
        _currentGroups = new List<ObjGroup>();
        foreach (var name in this.GroupNames)
        {
            ObjGroup? group = null;
            if (!Settings.HandleEachGroupOccurrenceAsNewGroup)
                group = GetGroupByName(name);

            if (group is null)
            {
                group = new ObjGroup(name);
                _obj.Groups.Add(group);
            }

            _currentGroups.Add(group);
        }

        if (_currentGroups.Count == 0)
        {
            _currentGroups.Add(_obj.DefaultGroup);
        }
    }

    private ObjGroup? GetGroupByName(string name)
        => _obj.Groups.Find(t => string.Equals(t.Name, name, StringComparison.Ordinal));

    public void ApplyAttributesToElement(ObjElement element)
    {
        element.ObjectName = this.ObjectName;
        element.LevelOfDetail = this.LevelOfDetail;
        element.MapName = this.MapName;
        element.MaterialName = this.MaterialName;
    }

    public void ApplyAttributesToPolygonalElement(ObjPolygonalElement element)
    {
        element.SmoothingGroupNumber = this.SmoothingGroupNumber;
        element.IsBevelInterpolationEnabled = this.IsBevelInterpolationEnabled;
        element.IsColorInterpolationEnabled = this.IsColorInterpolationEnabled;
        element.IsDissolveInterpolationEnabled = this.IsDissolveInterpolationEnabled;
    }

    public void ApplyAttributesToFreeFormElement(ObjFreeFormElement element)
    {
        element.MergingGroupNumber = this.MergingGroupNumber;
        element.FreeFormType = this.FreeFormType;
        element.IsRationalForm = this.IsRationalForm;
        element.DegreeU = this.DegreeU;
        element.DegreeV = this.DegreeV;
        element.BasicMatrixU = this.BasicMatrixU;
        element.BasicMatrixV = this.BasicMatrixV;
        element.StepU = this.StepU;
        element.StepV = this.StepV;
        element.CurveApproximationTechnique = this.CurveApproximationTechnique;
        element.SurfaceApproximationTechnique = this.SurfaceApproximationTechnique;
    }
}