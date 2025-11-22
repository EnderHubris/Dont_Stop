using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ChunkManager : Singleton<ChunkManager>
{
    [SerializeField] int MaxRenderDistance = 35, recursiveMax = 100;
    Dictionary<Vector3,Chunk> chunks = new Dictionary<Vector3, Chunk>();
    Chunk chunkWithPlayer = null;
    int chunksVisited = 0;

    void FixedUpdate()
    {
        chunksVisited = 0;
        if (chunkWithPlayer != null)
        {
            chunkWithPlayer.TestChunk(-1);
        }
    }

    public void AddChunk(Chunk chunk)
    {
        // check if a chunk being added is conflicting with another chunk
        bool confliction = !(chunks.TryAdd(chunk.position, chunk));
        if (confliction)
        {
            // if there is a confliction we need to swap the chunk at
            // a given position for a chunk that its neighbors are compatable with
            Chunk conflictingChunk = chunks[chunk.position];

            if (conflictingChunk.isOrigin)
            {
                Debug.Log("+++ Confliction with Origin Chunk! +++");
                chunk.Delete();
                return;
            }

            // grab reference to both chunks we need to connect
            Chunk chunkA = chunk.parent; // parent of newly spawned chunk
            Chunk chunkB = conflictingChunk.parent; // parent of chunk being conflicted by new chunk

            // check if conflicting chunk has a child and track a refernce to it
            Chunk childPresent = conflictingChunk.child;

            // remove both child chunks
            chunk.Delete();
            conflictingChunk.Delete();

            if (childPresent == null)
            {
                // find a chunk that is compatable with chunks A and B
            } else
                {        
                    // if child is present they need to be considered when
                    // finding a compatable chunk between chunks A and B
                }
        }
    }

    public int GetMaxRenderDistance() => MaxRenderDistance;

    // Update Manager with Chunk that contains the Player
    public void Signal(Chunk chunk)
    {
        chunkWithPlayer = chunk;
    }
    public void SignalVisit()
    {
        ++chunksVisited;
        if (chunksVisited > recursiveMax)
        {
            // Exit Play Mode in Editor
            #if UNITY_EDITOR
                Debug.LogError("Potential Stack Overflow from Recursion Occured!");
                EditorApplication.isPlaying = false;
            #endif
        }
    }

    void OnDrawGizmos()
    {
        if (chunkWithPlayer != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(chunkWithPlayer.transform.position, 5f);
        }
    }
}//EndScript