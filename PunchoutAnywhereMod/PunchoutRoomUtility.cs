using UnityEngine;
using Dungeonator;
using Pathfinding;
using tk2dRuntime.TileMap;
using System.Collections.Generic;
using System;

namespace PunchoutAnywhereMod {

    public class PunchoutRoomUtility : MonoBehaviour {

        private static PunchoutRoomUtility m_instance;
        public static PunchoutRoomUtility Instance {
            get {
                if (!m_instance) {
                    m_instance = ETGModMainBehaviour.Instance.gameObject.AddComponent<PunchoutRoomUtility>();
                }
                return m_instance;
            }
        }


        public static AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
        public static AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");

        private static PrototypeDungeonRoom non_elevator_entrance = assetBundle2.LoadAsset("non elevator entrance") as PrototypeDungeonRoom;
        private static PrototypeDungeonRoom RatChestHubRoom = assetBundle.LoadAsset("challengeshrine_castle_001") as PrototypeDungeonRoom;
        private static PrototypeDungeonRoom PunchoutMinigameRoom = assetBundle.LoadAsset("challengeshrine_gungeon_001") as PrototypeDungeonRoom;
        private static PrototypeDungeonRoom exit_room_basic = assetBundle2.LoadAsset("exit_room_basic") as PrototypeDungeonRoom;
        private static PrototypeDungeonRoom basic_secret_room_012 = assetBundle2.LoadAsset("basic_secret_room_012") as PrototypeDungeonRoom;
        private static PrototypeDungeonRoom high_dragunfire_room_001 = assetBundle2.LoadAsset("high_dragunfire_room_001") as PrototypeDungeonRoom;


        public static PrototypeDungeonRoom[] prototypeDungeonRoomArray = new PrototypeDungeonRoom[] {
            Instantiate(exit_room_basic),
            Instantiate(RatChestHubRoom),
            Instantiate(exit_room_basic),
            Instantiate(basic_secret_room_012),
            Instantiate(high_dragunfire_room_001),
            Instantiate(PunchoutMinigameRoom)
        };

        public RoomHandler[] GeneratePunchoutRoomCluster(Action<RoomHandler> postProcessCellData = null, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.FORCE_COLOR) {
            Dungeon dungeon = GameManager.Instance.Dungeon;

            IntVector2[] basePositions = new IntVector2[] {
                IntVector2.Zero,
                new IntVector2(10, 22),
                new IntVector2(39, 0),
                new IntVector2(51, 31),
                new IntVector2(73, 38),
                new IntVector2(10, 50)
            };

            prototypeDungeonRoomArray[1].name = "PunchoutReward";
            prototypeDungeonRoomArray[5].name = "PunchoutEntrance";
            prototypeDungeonRoomArray[3].name = "PunchoutFireGunSecret";
            prototypeDungeonRoomArray[4].name = "PunchoutBabyDragunSecret";

            GameObject tileMapObject = GameObject.Find("TileMap");
            tk2dTileMap m_tilemap = tileMapObject.GetComponent<tk2dTileMap>();

            if (m_tilemap == null) {
                ETGModConsole.Log("ERROR: TileMap object is null! Something seriously went wrong!");
                return null;
            }

            TK2DDungeonAssembler assembler = new TK2DDungeonAssembler();
            assembler.Initialize(dungeon.tileIndices);

            if (prototypeDungeonRoomArray.Length != basePositions.Length) {
                Debug.LogError("Attempting to add a malformed room cluster at runtime!");
                return null;
            }

            RoomHandler[] RoomClusterArray = new RoomHandler[prototypeDungeonRoomArray.Length];
            int num = 6;
            int num2 = 3;
            IntVector2 intVector = new IntVector2(int.MaxValue, int.MaxValue);
            IntVector2 intVector2 = new IntVector2(int.MinValue, int.MinValue);
            for (int i = 0; i < prototypeDungeonRoomArray.Length; i++) {
                intVector = IntVector2.Min(intVector, basePositions[i]);
                intVector2 = IntVector2.Max(intVector2, basePositions[i] + new IntVector2(prototypeDungeonRoomArray[i].Width, prototypeDungeonRoomArray[i].Height));
            }
            IntVector2 a = intVector2 - intVector;
            IntVector2 b = IntVector2.Min(IntVector2.Zero, -1 * intVector);
            a += b;
            IntVector2 intVector3 = new IntVector2(dungeon.data.Width + num, num);
            int newWidth = dungeon.data.Width + num * 2 + a.x;
            int newHeight = Mathf.Max(dungeon.data.Height, a.y + num * 2);
            CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
            dungeon.data.cellData = array;
            dungeon.data.ClearCachedCellData();
            for (int j = 0; j < prototypeDungeonRoomArray.Length; j++) {
                IntVector2 d = new IntVector2(prototypeDungeonRoomArray[j].Width, prototypeDungeonRoomArray[j].Height);
                IntVector2 b2 = basePositions[j] + b;
                IntVector2 intVector4 = intVector3 + b2;
                CellArea cellArea = new CellArea(intVector4, d, 0);
                cellArea.prototypeRoom = prototypeDungeonRoomArray[j];
                RoomHandler SelectedRoomInArray = new RoomHandler(cellArea);
                for (int k = -num; k < d.x + num; k++) {
                    for (int l = -num; l < d.y + num; l++) {
                        IntVector2 p = new IntVector2(k, l) + intVector4;
                        if ((k >= 0 && l >= 0 && k < d.x && l < d.y) || array[p.x][p.y] == null) {
                            CellData cellData = new CellData(p, CellType.WALL);
                            cellData.positionInTilemap = cellData.positionInTilemap - intVector3 + new IntVector2(num2, num2);
                            cellData.parentArea = cellArea;
                            cellData.parentRoom = SelectedRoomInArray;
                            cellData.nearestRoom = SelectedRoomInArray;
                            cellData.distanceFromNearestRoom = 0f;
                            array[p.x][p.y] = cellData;
                        }
                    }
                }
                dungeon.data.rooms.Add(SelectedRoomInArray);
                RoomClusterArray[j] = SelectedRoomInArray;
            }
            
            ConnectClusteredPunchoutRooms(RoomClusterArray[2], RoomClusterArray[1], prototypeDungeonRoomArray[2], prototypeDungeonRoomArray[1], 2, 2, 5, 5);
            ConnectClusteredPunchoutRooms(RoomClusterArray[3], RoomClusterArray[1], prototypeDungeonRoomArray[3], prototypeDungeonRoomArray[1], 0, 8, 1, 0);
            ConnectClusteredPunchoutRooms(RoomClusterArray[4], RoomClusterArray[3], prototypeDungeonRoomArray[4], prototypeDungeonRoomArray[3], 0, 3, 0, 1);            

            for (int n = 0; n < RoomClusterArray.Length; n++) {
                try {
                    RoomClusterArray[n].WriteRoomData(dungeon.data);
                } catch (Exception) {
                    ETGModConsole.Log("WARNING: Exception caused during WriteRoomData step on room: " + RoomClusterArray[n].GetRoomName());
                } try {
                    dungeon.data.GenerateLightsForRoom(dungeon.decoSettings, RoomClusterArray[n], GameObject.Find("_Lights").transform, lightStyle);
                } catch (Exception) {
                    ETGModConsole.Log("WARNING: Exception caused during GeernateLightsForRoom step on room: " + RoomClusterArray[n].GetRoomName());
                }
                postProcessCellData?.Invoke(RoomClusterArray[n]);
                if (RoomClusterArray[n].area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.SECRET) {
                    RoomClusterArray[n].BuildSecretRoomCover();
                }
            }
            GameObject gameObject = (GameObject)Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
            tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
            string str = UnityEngine.Random.Range(10000, 99999).ToString();
            gameObject.name = "Punchout_" + "RuntimeTilemap_" + str;
            component.renderData.name = "Punchout_" + "RuntimeTilemap_" + str + " Render Data";
            component.Editor__SpriteCollection = dungeon.tileIndices.dungeonCollection;
            //
            // Create Pits in Entrance rooom used for Punchout fight.
            // Must run before certain steps in room generation else the pits won't be "visible" to the player.
            PitStamper(RoomClusterArray[5], new IntVector2(14, 20));
            RoomClusterArray[5].TargetPitfallRoom = RoomClusterArray[1];
            RoomClusterArray[5].ForcePitfallForFliers = true;
            RoomClusterArray[4].TargetPitfallRoom = RoomClusterArray[3];
            TK2DDungeonAssembler.RuntimeResizeTileMap(component, a.x + num2 * 2, a.y + num2 * 2, m_tilemap.partitionSizeX, m_tilemap.partitionSizeY);
            for (int num3 = 0; num3 < prototypeDungeonRoomArray.Length; num3++) {
                IntVector2 intVector5 = new IntVector2(prototypeDungeonRoomArray[num3].Width, prototypeDungeonRoomArray[num3].Height);
                IntVector2 b3 = basePositions[num3] + b;
                IntVector2 intVector6 = intVector3 + b3;
                for (int num4 = -num2; num4 < intVector5.x + num2; num4++) {
                    for (int num5 = -num2; num5 < intVector5.y + num2 + 2; num5++) {
                        assembler.BuildTileIndicesForCell(dungeon, component, intVector6.x + num4, intVector6.y + num5);
                    }
                }
            }
            RenderMeshBuilder.CurrentCellXOffset = intVector3.x - num2;
            RenderMeshBuilder.CurrentCellYOffset = intVector3.y - num2;
            component.ForceBuild();
            RenderMeshBuilder.CurrentCellXOffset = 0;
            RenderMeshBuilder.CurrentCellYOffset = 0;
            component.renderData.transform.position = new Vector3(intVector3.x - num2, intVector3.y - num2, intVector3.y - num2);
            for (int num6 = 0; num6 < RoomClusterArray.Length; num6++) {
                RoomClusterArray[num6].OverrideTilemap = component;
                for (int num7 = 0; num7 < RoomClusterArray[num6].area.dimensions.x; num7++) {
                    for (int num8 = 0; num8 < RoomClusterArray[num6].area.dimensions.y + 2; num8++) {
                        IntVector2 intVector7 = RoomClusterArray[num6].area.basePosition + new IntVector2(num7, num8);
                        if (dungeon.data.CheckInBoundsAndValid(intVector7)) {
                             CellData currentCell = dungeon.data[intVector7];
                            TK2DInteriorDecorator.PlaceLightDecorationForCell(dungeon, component, currentCell, intVector7);
                        }
                    }
                }
                Pathfinder.Instance.InitializeRegion(dungeon.data, RoomClusterArray[num6].area.basePosition + new IntVector2(-3, -3), RoomClusterArray[num6].area.dimensions + new IntVector2(3, 3));
                if (!RoomClusterArray[num6].IsSecretRoom && RoomClusterArray[num6] != RoomClusterArray[0] && RoomClusterArray[num6] != RoomClusterArray[2]) {
                    RoomClusterArray[num6].RevealedOnMap = true;
                    RoomClusterArray[num6].visibility = RoomHandler.VisibilityStatus.VISITED;
                    StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(RoomClusterArray[num6], true, true, false));
                }
                if (RoomClusterArray[num6] == RoomClusterArray[0] | RoomClusterArray[num6] == RoomClusterArray[2]) {
                    StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(RoomClusterArray[num6], true, true, false));
                    RoomClusterArray[num6].RevealedOnMap = false;
                    RoomClusterArray[num6].visibility = RoomHandler.VisibilityStatus.REOBSCURED;
                }
                RoomClusterArray[num6].PostGenerationCleanup();
            }            
            DeadlyDeadlyGoopManager.ReinitializeData();
            Minimap.Instance.InitializeMinimap(dungeon.data);
            return RoomClusterArray;
        }             

        private void ConnectClusteredPunchoutRooms(RoomHandler first, RoomHandler second, PrototypeDungeonRoom firstPrototype, PrototypeDungeonRoom secondPrototype, int firstRoomExitIndex, int secondRoomExitIndex, int room1ExitLengthPadding = 3, int room2ExitLengthPadding = 3) {
            if (first.area.instanceUsedExits == null | second.area.exitToLocalDataMap == null |
                second.area.instanceUsedExits == null | first.area.exitToLocalDataMap == null)
            { return; }
            try {
                first.area.instanceUsedExits.Add(firstPrototype.exitData.exits[firstRoomExitIndex]);
                RuntimeRoomExitData runtimeRoomExitData = new RuntimeRoomExitData(firstPrototype.exitData.exits[firstRoomExitIndex]);
                first.area.exitToLocalDataMap.Add(firstPrototype.exitData.exits[firstRoomExitIndex], runtimeRoomExitData);
                second.area.instanceUsedExits.Add(secondPrototype.exitData.exits[secondRoomExitIndex]);
                RuntimeRoomExitData runtimeRoomExitData2 = new RuntimeRoomExitData(secondPrototype.exitData.exits[secondRoomExitIndex]);
                second.area.exitToLocalDataMap.Add(secondPrototype.exitData.exits[secondRoomExitIndex], runtimeRoomExitData2);
                first.connectedRooms.Add(second);
                first.connectedRoomsByExit.Add(firstPrototype.exitData.exits[firstRoomExitIndex], second);
                first.childRooms.Add(second);
                second.connectedRooms.Add(first);
                second.connectedRoomsByExit.Add(secondPrototype.exitData.exits[secondRoomExitIndex], first);
                second.parentRoom = first;
                runtimeRoomExitData.linkedExit = runtimeRoomExitData2;
                runtimeRoomExitData2.linkedExit = runtimeRoomExitData;
                runtimeRoomExitData.additionalExitLength = room1ExitLengthPadding;
                runtimeRoomExitData2.additionalExitLength = room2ExitLengthPadding;
            }
            catch (Exception) {
                ETGModConsole.Log("WARNING: Exception caused during CoonectClusteredRunTimeRooms method!");
                return;
            }
        }

        public void PitStamper(RoomHandler target, IntVector2 targetPosition) {
            IntVector2 pos = targetPosition + target.area.basePosition;
            target.RuntimeStampCellComplex(pos.X, pos.Y, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 1, pos.Y, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 2, pos.Y, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 3, pos.Y, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 4, pos.Y, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X, pos.Y - 1, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 1, pos.Y - 1, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 2, pos.Y - 1, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 3, pos.Y - 1, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 4, pos.Y - 1, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X, pos.Y - 2, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 1, pos.Y - 2, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 2, pos.Y - 2, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 3, pos.Y - 2, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 4, pos.Y - 2, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X, pos.Y - 3, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 1, pos.Y - 3, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 2, pos.Y - 3, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 3, pos.Y - 3, CellType.PIT, DiagonalWallType.NONE);
            target.RuntimeStampCellComplex(pos.X + 4, pos.Y - 3, CellType.PIT, DiagonalWallType.NONE);
        }
        
        public void PostProcessWallCleanup(RoomHandler target) {
            DungeonData data = GameManager.Instance.Dungeon.data;
            for (int i = 0; i < target.area.dimensions.x; i++) {
                for (int j = 0; j < target.area.dimensions.y + 2; j++) {
                    IntVector2 intVector = target.area.basePosition + new IntVector2(i, j);
                    if (data.CheckInBoundsAndValid(intVector)) {
                        CellData cellData = data[intVector];
                        if (data.isAnyFaceWall(intVector.x, intVector.y)) {
                            TilesetIndexMetadata.TilesetFlagType key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_UPPER;
                            if (data.isFaceWallLower(intVector.x, intVector.y)) {
                                key = TilesetIndexMetadata.TilesetFlagType.FACEWALL_LOWER;
                            }
                            int indexFromTupleArray = SecretRoomUtility.GetIndexFromTupleArray(cellData, SecretRoomUtility.metadataLookupTableRef[key], cellData.cellVisualData.roomVisualTypeIndex, 0f);
                            cellData.cellVisualData.faceWallOverrideIndex = indexFromTupleArray;
                        }
                    }
                }
            }
        }
        // Obsolete. I now use AddRuntimeRoomCluster instead. :D
        public RoomHandler AddPunchoutRuntimeRoom(PrototypeDungeonRoom prototype, bool addTeleporter = true, bool visibleOnMinimap = true, DungeonData.LightGenerationStyle lightStyle = DungeonData.LightGenerationStyle.STANDARD) {
            Dungeon dungeon = GameManager.Instance.Dungeon;

            GameObject tileMapObject = GameObject.Find("TileMap");
            tk2dTileMap m_tilemap = tileMapObject.GetComponent<tk2dTileMap>();

            if (m_tilemap == null) {
                ETGModConsole.Log("ERROR: tileMap object is null! Something seriously went wrong!");
                return null;
            }

            TK2DDungeonAssembler assembler = new TK2DDungeonAssembler();
            assembler.Initialize(dungeon.tileIndices);

            if (m_tilemap == null | assembler == null ) {
                ETGModConsole.Log("ERROR: Tilemap object and/or TK2DDungeonAseembler returned null!", false);
                return null;
            }

            int num = 6;
            int num2 = 3;
            IntVector2 d = new IntVector2(prototype.Width, prototype.Height);
			IntVector2 intVector = new IntVector2(dungeon.data.Width + num, num);
			int newWidth = dungeon.data.Width + num * 2 + d.x;
			int newHeight = Mathf.Max(dungeon.data.Height, d.y + num * 2);
			CellData[][] array = BraveUtility.MultidimensionalArrayResize(dungeon.data.cellData, dungeon.data.Width, dungeon.data.Height, newWidth, newHeight);
			CellArea cellArea = new CellArea(intVector, d, 0);
			cellArea.prototypeRoom = prototype;
            dungeon.data.cellData = array;
            dungeon.data.ClearCachedCellData();
			RoomHandler roomHandler = new RoomHandler(cellArea);
			for (int i = -num; i < d.x + num; i++) {
				for (int j = -num; j < d.y + num; j++) {
					IntVector2 p = new IntVector2(i, j) + intVector;
					CellData cellData = new CellData(p, CellType.WALL);
					cellData.positionInTilemap = cellData.positionInTilemap - intVector + new IntVector2(num2, num2);
					cellData.parentArea = cellArea;
					cellData.parentRoom = roomHandler;
					cellData.nearestRoom = roomHandler;
					cellData.distanceFromNearestRoom = 0f;
					array[p.x][p.y] = cellData;
				}
			}
			roomHandler.WriteRoomData(dungeon.data);
			for (int k = -num; k < d.x + num; k++) {
				for (int l = -num; l < d.y + num; l++) {
					IntVector2 intVector2 = new IntVector2(k, l) + intVector;
					array[intVector2.x][intVector2.y].breakable = true;
				}
			}
            dungeon.data.rooms.Add(roomHandler);
			GameObject gameObject = (GameObject)Instantiate(BraveResources.Load("RuntimeTileMap", ".prefab"));
            gameObject.name = "CustomRunTimeTileMap";
			tk2dTileMap component = gameObject.GetComponent<tk2dTileMap>();
			component.Editor__SpriteCollection = dungeon.tileIndices.dungeonCollection;
			dungeon.data.GenerateLightsForRoom(dungeon.decoSettings, roomHandler, GameObject.Find("_Lights").transform, lightStyle);
            
            TK2DDungeonAssembler.RuntimeResizeTileMap(component, d.x + num2 * 2, d.y + num2 * 2, m_tilemap.partitionSizeX, m_tilemap.partitionSizeY);
			for (int m = -num2; m < d.x + num2; m++) {
				for (int n = -num2; n < d.y + num2; n++) {
                    assembler.BuildTileIndicesForCell(dungeon, component, intVector.x + m, intVector.y + n);
				}
			}
			RenderMeshBuilder.CurrentCellXOffset = intVector.x - num2;
			RenderMeshBuilder.CurrentCellYOffset = intVector.y - num2;
			component.Build();
			RenderMeshBuilder.CurrentCellXOffset = 0;
			RenderMeshBuilder.CurrentCellYOffset = 0;
			component.renderData.transform.position = new Vector3(intVector.x - num2, intVector.y - num2, intVector.y - num2);
			roomHandler.OverrideTilemap = component;
			Pathfinder.Instance.InitializeRegion(dungeon.data, roomHandler.area.basePosition + new IntVector2(-3, -3), roomHandler.area.dimensions + new IntVector2(3, 3));
			roomHandler.PostGenerationCleanup();
			DeadlyDeadlyGoopManager.ReinitializeData();

            if (addTeleporter) { roomHandler.AddProceduralTeleporterToRoom(); }

            if (visibleOnMinimap) {
                roomHandler.visibility = RoomHandler.VisibilityStatus.VISITED;
                StartCoroutine(Minimap.Instance.RevealMinimapRoomInternal(roomHandler, true, true, false));
                Minimap.Instance.InitializeMinimap(dungeon.data);
            }

            return roomHandler;
		}
    }
}

