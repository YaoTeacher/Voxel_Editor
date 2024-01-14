using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Structure {

    public static Queue<VoxelMod> MakeTree (Vector3Int index, int minTrunkHeight, int maxTrunkHeight) {

        Queue<VoxelMod> queue = new Queue<VoxelMod>();

        int height = (int)(maxTrunkHeight * Noise.Get2DPerlin(new Vector2(index.x, index.z), 250f, 3f));

        if (height < minTrunkHeight)
            height = minTrunkHeight;

        for (int i = 1; i < height; i++)
            queue.Enqueue(new VoxelMod(new Vector3Int(index.x, index.y + i, index.z), 6));

        for (int x = -3; x < 4; x++) {
            for (int y = 0; y < 7; y++) {
                for (int z = -3; z < 4; z++) {
                    queue.Enqueue(new VoxelMod(new Vector3Int(index.x + x, index.y + height + y, index.z + z), 11));
                }
            }
        }

        return queue;

    }



}
