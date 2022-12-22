using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PathCreation;
using UnityEngine;

public enum PathType
{
    RELATIVE,
    ABSOLUTE,
}

public enum PathGroup
{
    BULLET,
    NO_BULLET,
}
public class PathObject : MonoBehaviour
{
    public PathCreator Path => m_PathCreator;
    public PathType PathType => m_PathType;
    public PathGroup Group => m_PathGroup;
    public Vector2 DeltaPosition => m_DeltaPosition;
    
    [SerializeField] private PathType m_PathType = PathType.ABSOLUTE;
    [SerializeField] private PathGroup m_PathGroup = PathGroup.NO_BULLET;
    [SerializeField] private PathCreator m_PathCreator;
    [SerializeField] private Vector2 m_DeltaPosition = Vector2.zero;
    [SerializeField] private Vector2 m_DeltaRotation = Vector2.zero;

    private void OnValidate()
    {
        m_PathCreator = GetComponent<PathCreator>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }
}
