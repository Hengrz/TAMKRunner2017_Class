﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{

    public Transform[] m_aFloorPrefabs;

    public float m_fFloorStartZ;
    public float m_fFloorEndZ;
    public float m_fMinMovementSpeed;
    public float m_fMaxMovementSpeed;

    public float m_fSpeedupInterval = 1.0f;
    public float m_fSpeedUpAmount;

    private float m_fMovementSpeed;
    private int m_iNumberOfParts;
    private float m_fEventTime;


    // Use this for initialization
    void Start()
    {
        float fZPos = 0.0f;
        float fFloorLength = Mathf.Abs(m_fFloorStartZ) + Mathf.Abs(m_fFloorEndZ);
        m_fMovementSpeed = m_fMinMovementSpeed;

        Debug.Log("Floor length is: " + fFloorLength.ToString());

        while (fZPos > -(fFloorLength - 15))
        {
            SpawnNewFloor(fZPos);
            fZPos -= 5.0f;
        }

        SpawnNewFloor(fZPos, false);
        fZPos -= 5.0f;
        SpawnNewFloor(fZPos, false);
        fZPos -= 5.0f;
        SpawnNewFloor(fZPos, false);
        fZPos -= 5.0f;

        m_fEventTime = Time.time;
    }


    public void SpawnNewFloor(float fZOffset, bool bRand = true)
    {
        Transform tFloorTransform = null;

        if (bRand)
            tFloorTransform = Instantiate(m_aFloorPrefabs[Random.Range(0, m_aFloorPrefabs.Length)], new Vector3(0.0f, 0.0f, m_fFloorStartZ + fZOffset), Quaternion.identity);
        else
            tFloorTransform = Instantiate(m_aFloorPrefabs[0], new Vector3(0.0f, 0.0f, m_fFloorStartZ + fZOffset), Quaternion.identity);

        // Check we spawned something...
        if (null == tFloorTransform)
        {
            Debug.LogError("Unable to instantiate floor part");
            return;
        }

        // Get and check for the floor part controller
        FloorPart gcFloorPart = tFloorTransform.GetComponent<FloorPart>();
        if (null == gcFloorPart)
        {
            Debug.LogError("Prefab does not have a FloorPart component, unable to create the floor!");
            return;
        }

        // Set default params
        gcFloorPart.m_fEndZ = m_fFloorEndZ;
        gcFloorPart.m_fStartZ = m_fFloorStartZ + fZOffset;
        gcFloorPart.m_fMovementSpeed = m_fMovementSpeed;

        gcFloorPart.m_gcFloorManager = this;
    }


    // Update is called once per frame
    void Update()
    {
        if (Time.time - m_fEventTime > m_fSpeedupInterval)
        {
            m_fMovementSpeed += Mathf.Clamp(m_fMovementSpeed + m_fSpeedUpAmount, m_fMinMovementSpeed, m_fMaxMovementSpeed);
            m_fEventTime = Time.time;

            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Floor Parts"))
            {
                FloorPart gcFloorPart = go.GetComponent<FloorPart>();
                if (null != gcFloorPart)
                    gcFloorPart.m_fMovementSpeed = m_fMovementSpeed;
            }
        }
    }
}
