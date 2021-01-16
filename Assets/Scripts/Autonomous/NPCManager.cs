﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NPCManager : MonoBehaviour
{
    private ISet<GameObject> m_NpcSet;
    public const float NPC_RUNAWAY_RANGE = 20.0f;
    public const float NPC_RUN_DIST = 60.0f;

    public static NPCManager Instance
    {
        get; private set;
    }

    public void NotifyCrashSite(Vector3 hitPosition)
    {
        Debug.Log(hitPosition);
        IEnumerable<(NPC, Vector3)> npcsInRange;
        lock (this)
        {
            // find all NPCs within range
            npcsInRange =
               from npc in m_NpcSet
               where Vector3.Distance(npc.transform.position, hitPosition) <= NPC_RUNAWAY_RANGE
               select (npc.GetComponent<NPC>(), npc.transform.position);
        }
        foreach ((NPC npc, Vector3 npcPos) in npcsInRange)
        {
            Vector3 dirToRun = (npcPos - hitPosition).normalized;
            Debug.Log(dirToRun);
            Debug.DrawRay(npc.transform.position, dirToRun, Color.green, 10.0f);
            Vector3 target = npcPos + dirToRun * NPC_RUN_DIST;
            target.y = npc.transform.position.y;
            npc.Target = target;
        }
    }

    void Start()
    {
        lock (this)
        {
            GameObject[] npcs = GameObject.FindGameObjectsWithTag("NPC");
            m_NpcSet = new HashSet<GameObject>();
            foreach (GameObject npc in npcs)
            {
                m_NpcSet.Add(npc);
            }
        }
        Instance = this;
    }

    void Update()
    {

    }
}
