using Dungeonator;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PunchoutAnywhereMod {
    
    class PunchoutInit : MonoBehaviour {

        public RoomHandler[] PunchoutRoomCluster;

        private void Start() {
            ETGModConsole.Commands.AddGroup("playpunchout", delegate (string[] e) {
                var levelOverrideState = GameManager.Instance.CurrentLevelOverrideState;

                if (levelOverrideState == GameManager.LevelOverrideState.FOYER) {
                    ETGModConsole.Log("ERROR: Punchout should not be started from the Breach!\nPlease select a character and start a run first!");
                    return;
                } try {
                    InitPunchOutRoomCluster();
                    StartCoroutine(StartPunchout());
                } catch (Exception Ex) {
                    ETGModConsole.Log("ERROR: Exception during Room Cluster setup! Try again on a different floor!");
                    Debug.Log("ERROR: Exception during Room Cluster setup! Try again on a different floor!");
                    Debug.LogException(Ex);
                }                
            });
        }

        private void InitPunchOutRoomCluster() {
            Dungeon dungeon = GameManager.Instance.Dungeon;
            PlayerController PrimaryPlayer = GameManager.Instance.PrimaryPlayer;

            int CurrentFloor = GameManager.Instance.CurrentFloor;


            PunchoutRoomCluster = PunchoutRoomUtility.Instance.GeneratePunchoutRoomCluster();
            
            if (CurrentFloor == 1 && PrimaryPlayer.CurrentRoom.area.PrototypeRoomCategory == PrototypeDungeonRoom.RoomCategory.ENTRANCE) {
                PrimaryPlayer.CurrentRoom.AddProceduralTeleporterToRoom();
            }
            
            PlaceRatObjects(PunchoutRoomCluster);

            PrimaryPlayer.EscapeRoom(PlayerController.EscapeSealedRoomStyle.TELEPORTER, true, PunchoutRoomCluster[5]);
            PrimaryPlayer.WarpFollowersToPlayer();

            if (GameManager.Instance.CurrentGameType == GameManager.GameType.COOP_2_PLAYER) {
                PlayerController SecondaryPlayer = GameManager.Instance.SecondaryPlayer;
                GameManager.Instance.GetOtherPlayer(SecondaryPlayer).ReuniteWithOtherPlayer(PrimaryPlayer, false);
            }
        }

        private void PlaceRatObjects(RoomHandler[] targetRooms) {
            int CurrentFloor = GameManager.Instance.CurrentFloor;
            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("shared_auto_001");
            AssetBundle assetBundle2 = ResourceManager.LoadAssetBundle("shared_auto_002");
            IntVector2[] RoomOffsets = new IntVector2[] {
                 targetRooms[0].area.basePosition,
                 targetRooms[1].area.basePosition,
                 targetRooms[2].area.basePosition,
                 targetRooms[3].area.basePosition,
                 targetRooms[4].area.basePosition,
                 targetRooms[5].area.basePosition,
            };
            GameObject ChestRat = assetBundle.LoadAsset("Chest_Rat") as GameObject;
            GameObject RewardPedestalPrefab = assetBundle.LoadAsset("Boss_Reward_Pedestal") as GameObject;
            GameObject InfoSign = assetBundle2.LoadAsset("teleporter_info_sign") as GameObject;
            DungeonPlaceable ChestPlatform = assetBundle2.LoadAsset("Treasure_Dais_Stone_Carpet") as DungeonPlaceable;
            DungeonPlaceable ElevatorDeparture = assetBundle2.LoadAsset("Elevator_Departure") as DungeonPlaceable;
            Chest RatChest = ChestRat.GetComponent<Chest>();
            GameObject RewardPedestalPlatform = null;

            WeightedGameObject wChestObject = new WeightedGameObject();
            wChestObject.rawGameObject = ChestRat;
            WeightedGameObjectCollection wChestObjectCollection = new WeightedGameObjectCollection();
            wChestObjectCollection.Add(wChestObject);
            Chest PlacedRatChest1 = targetRooms[1].SpawnRoomRewardChest(wChestObjectCollection, RoomOffsets[1] + new IntVector2(3, 18));
            Chest PlacedRatChest2 = targetRooms[1].SpawnRoomRewardChest(wChestObjectCollection, RoomOffsets[1] + new IntVector2(5, 12));
            Chest PlacedRatChest3 = targetRooms[1].SpawnRoomRewardChest(wChestObjectCollection, RoomOffsets[1] + new IntVector2(26, 12));
            Chest PlacedRatChest4 = targetRooms[1].SpawnRoomRewardChest(wChestObjectCollection, RoomOffsets[1] + new IntVector2(28, 18));
            GameObject ChestPlatform1 = ChestPlatform.InstantiateObject(targetRooms[1], new IntVector2(3, 17));
            GameObject ChestPlatform2 = ChestPlatform.InstantiateObject(targetRooms[1], new IntVector2(5, 11));
            GameObject ChestPlatform3 = ChestPlatform.InstantiateObject(targetRooms[1], new IntVector2(26, 11));
            GameObject ChestPlatform4 = ChestPlatform.InstantiateObject(targetRooms[1], new IntVector2(28, 17));
            GameObject ChestPlatform5 = ChestPlatform.InstantiateObject(targetRooms[3], new IntVector2(6, 6));
            GameObject RewardPedestal1 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(12, 16)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal2 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(12, 12)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal3 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(12, 8)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal4 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(12, 4)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal5 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(22, 16)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal6 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(22, 12)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal7 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(22, 8)).ToVector3(), Quaternion.identity);
            GameObject RewardPedestal8 = Instantiate(RewardPedestalPrefab, (RoomOffsets[1] + new IntVector2(22, 4)).ToVector3(), Quaternion.identity);

            RewardPedestal RewardPedestalComponent1 = RewardPedestal1.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent2 = RewardPedestal2.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent3 = RewardPedestal3.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent4 = RewardPedestal4.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent5 = RewardPedestal5.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent6 = RewardPedestal6.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent7 = RewardPedestal7.GetComponent<RewardPedestal>();
            RewardPedestal RewardPedestalComponent8 = RewardPedestal8.GetComponent<RewardPedestal>();
            RewardPedestalComponent1.SpecificItemId = 600;
            RewardPedestalComponent1.SpawnsTertiarySet = false;
            RewardPedestalComponent1.UsesSpecificItem = true;
            RewardPedestalComponent1.overrideMimicChance = 0f;            
            RewardPedestalComponent2.SpecificItemId = 120;
            RewardPedestalComponent2.SpawnsTertiarySet = false;
            RewardPedestalComponent2.UsesSpecificItem = true;
            RewardPedestalComponent2.overrideMimicChance = 0f;            
            RewardPedestalComponent3.SpecificItemId = 565;
            RewardPedestalComponent3.SpawnsTertiarySet = false;
            RewardPedestalComponent3.UsesSpecificItem = true;
            RewardPedestalComponent3.overrideMimicChance = 0f;            
            RewardPedestalComponent4.SpecificItemId = 67;
            RewardPedestalComponent4.SpawnsTertiarySet = false;
            RewardPedestalComponent4.UsesSpecificItem = true;
            RewardPedestalComponent4.overrideMimicChance = 0f;
            RewardPedestalComponent5.SpecificItemId = 78;
            RewardPedestalComponent5.SpawnsTertiarySet = false;
            RewardPedestalComponent5.UsesSpecificItem = true;
            RewardPedestalComponent5.overrideMimicChance = 0f;
            RewardPedestalComponent6.SpecificItemId = 85;
            RewardPedestalComponent6.SpawnsTertiarySet = false;
            RewardPedestalComponent6.UsesSpecificItem = true;
            RewardPedestalComponent6.overrideMimicChance = 0f;
            RewardPedestalComponent7.SpecificItemId = 224;
            RewardPedestalComponent7.SpawnsTertiarySet = false;
            RewardPedestalComponent7.UsesSpecificItem = true;
            RewardPedestalComponent7.overrideMimicChance = 0f;
            RewardPedestalComponent8.SpecificItemId = 67;
            RewardPedestalComponent8.SpawnsTertiarySet = false;
            RewardPedestalComponent8.UsesSpecificItem = true;
            RewardPedestalComponent8.overrideMimicChance = 0f;

            DungeonPlaceableBehaviour[] DungeonPlacables = FindObjectsOfType<DungeonPlaceableBehaviour>();
            ElevatorDepartureController[] ExitElevators = FindObjectsOfType<ElevatorDepartureController>();
            TeleporterController[] ExitRoomTeleporters = FindObjectsOfType<TeleporterController>();
            ChallengeShrineController[] cachedChallengeShrineObjects = FindObjectsOfType<ChallengeShrineController>();
            TalkDoerLite[] NPCObjects = FindObjectsOfType<TalkDoerLite>();
            Chest[] ChestObjects = FindObjectsOfType<Chest>();
            GameObject[] gameObjects = FindObjectsOfType<GameObject>();


            if(gameObjects.Length > 0) {
                for (int i = 0; i < gameObjects.Length; i++) {
                    if (gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[1] && gameObjects[i].name.ToLower().StartsWith("treasure_dais_stone_carpet")) {
                        RewardPedestalPlatform = gameObjects[i];
                        break;
                    }
                }
                for (int i = 0; i < gameObjects.Length; i++) {
                    if (gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[1] && gameObjects[i].name.ToLower().StartsWith("spotlight")) {
                        Destroy(gameObjects[i]);
                    }
                }
                for (int i = 0; i < gameObjects.Length; i++) {
                    if (gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[1] | gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[5] && gameObjects[i].name.ToLower().StartsWith("godray")) {
                        Destroy(gameObjects[i]);
                    }
                }
            }

            if (RewardPedestalPlatform != null) {
                GameObject PlacedRewardPedestalPlatform1 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(11, 15)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform2 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(11, 11)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform3 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(11, 7)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform4 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(11, 3)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform5 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(21, 15)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform6 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(21, 11)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform7 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(21, 7)).ToVector3(), Quaternion.identity);
                GameObject PlacedRewardPedestalPlatform8 = Instantiate(RewardPedestalPlatform, (RoomOffsets[1] + new IntVector2(21, 3)).ToVector3(), Quaternion.identity);
                tk2dBaseSprite[] platformsprites1 = PlacedRewardPedestalPlatform1.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites2 = PlacedRewardPedestalPlatform2.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites3 = PlacedRewardPedestalPlatform3.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites4 = PlacedRewardPedestalPlatform4.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites5 = PlacedRewardPedestalPlatform5.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites6 = PlacedRewardPedestalPlatform6.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites7 = PlacedRewardPedestalPlatform7.GetComponents<tk2dBaseSprite>();
                tk2dBaseSprite[] platformsprites8 = PlacedRewardPedestalPlatform8.GetComponents<tk2dBaseSprite>();
                if (platformsprites1 != null && platformsprites1.Length > 0 &&
                    platformsprites2 != null && platformsprites2.Length > 0 &&
                    platformsprites3 != null && platformsprites3.Length > 0 &&
                    platformsprites4 != null && platformsprites4.Length > 0 &&
                    platformsprites5 != null && platformsprites5.Length > 0 &&
                    platformsprites6 != null && platformsprites6.Length > 0 &&
                    platformsprites7 != null && platformsprites7.Length > 0 &&
                    platformsprites8 != null && platformsprites8.Length > 0)
                {
                    for (int i = 0; i < platformsprites1.Length; i++) {
                        platformsprites1[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites2.Length; i++) {
                        platformsprites2[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites3.Length; i++) {
                        platformsprites3[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites4.Length; i++) {
                        platformsprites4[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites5.Length; i++) {
                        platformsprites5[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites6.Length; i++) {
                        platformsprites6[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites7.Length; i++) {
                        platformsprites7[i].scale = new Vector3(0.9f, 0.9f);
                    }
                    for (int i = 0; i < platformsprites8.Length; i++) {
                        platformsprites8[i].scale = new Vector3(0.9f, 0.9f);
                    }
                }                
            }

            // Change names of placables to allow TargetPitFall room to read them as "Arrival".
            // This allows rat corpse to properly teleport to destination room when droppped in pit.
            // Also used to prevent chance of player being spawned inside a rat chest object if it used the random placement method.
            if (DungeonPlacables.Length > 0) {
                for (int i = 0; i < DungeonPlacables.Length; i++) {
                    if (DungeonPlacables[i].transform.position.GetAbsoluteRoom() == targetRooms[1] && DungeonPlacables[i].name.ToLower().StartsWith("shrine")) {
                        DungeonPlacables[i].gameObject.name = "Arrival";
                        DungeonPlacables[i].transform.name = "Arrival";
                    }
                    if (DungeonPlacables[i].transform.position.GetAbsoluteRoom() == targetRooms[3] && DungeonPlacables[i].name.ToLower().StartsWith("godray")) {
                        DungeonPlacables[i].gameObject.name = "Arrival";
                        DungeonPlacables[i].transform.name = "Arrival";
                    }
                }
            }

            if (cachedChallengeShrineObjects.Length > 0) {
                for (int i = 0; i < cachedChallengeShrineObjects.Length; i++) {
                    if (cachedChallengeShrineObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[1] | cachedChallengeShrineObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[5]) {
                        cachedChallengeShrineObjects[i].GetRidOfMinimapIcon();
                        Destroy(cachedChallengeShrineObjects[i].gameObject);
                    }
                }
            }

            GameObject PlacedInfoSign = Instantiate(InfoSign, RoomOffsets[5].ToVector3() + new Vector3(12, 14), Quaternion.identity);
            if (PlacedInfoSign != null) {
                NoteDoer PlacedInfoSignNoteDoer = PlacedInfoSign.GetComponent<NoteDoer>();
                if (PlacedInfoSignNoteDoer != null) {
                    PlacedInfoSignNoteDoer.stringKey = "Use this pit to advance to the next room. If you won the Punchout minigame, also kick Rat Corpse into this pit.";
                    PlacedInfoSignNoteDoer.useAdditionalStrings = false;
                    PlacedInfoSignNoteDoer.alreadyLocalized = true;
                    targetRooms[5].RegisterInteractable(PlacedInfoSignNoteDoer);
                }
            }
           
            if (CurrentFloor <= 3 | CurrentFloor == -1) {
                if (ExitElevators.Length > 0) {
                    for (int i = 0; i < ExitElevators.Length; i++) {
                        if (ExitElevators[i].transform.position.GetAbsoluteRoom() == targetRooms[0]) {
                            ExitElevators[i].UsesOverrideTargetFloor = true;
                            ExitElevators[i].OverrideTargetFloor = GlobalDungeonData.ValidTilesets.CATACOMBGEON;
                            break;
                        }
                    }
                }
            }

            if (CurrentFloor <= 4 | CurrentFloor == -1) {
                if (ExitElevators.Length > 0) {
                    for (int i = 0; i < ExitElevators.Length; i++) {
                        if (ExitElevators[i].transform.position.GetAbsoluteRoom() == targetRooms[2]) {
                            ExitElevators[i].UsesOverrideTargetFloor = true;
                            ExitElevators[i].OverrideTargetFloor = GlobalDungeonData.ValidTilesets.FORGEGEON;
                            break;
                        }
                    }
                }
            }
            
            List<int> cachedRatChestItem1 = new List<int>() {626}; // elimentaler
            List<int> cachedRatChestItem2 = new List<int>() {662}; // partially_eaten_cheese
            List<int> cachedRatChestItem3 = new List<int>() {663}; // resourceful_sack
            List<int> cachedRatChestItem4 = new List<int>() {667}; // rat_boots

            List<int> cachedRatChestEgg = new List<int>() { 637 }; // wierd_egg
            List<int> cachedRatChestFire = new List<int>() { 196 }; // fossilized_gun

            PlacedRatChest1.forceContentIds = cachedRatChestItem1;
            PlacedRatChest2.forceContentIds = cachedRatChestItem2;
            PlacedRatChest3.forceContentIds = cachedRatChestItem3;
            PlacedRatChest4.forceContentIds = cachedRatChestItem4;

            if (ChestObjects.Length > 0 && gameObjects.Length > 0) {
                for (int i = 0; i < ChestObjects.Length; i++) {
                    if (ChestObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[3] && ChestObjects[i].ChestIdentifier != Chest.SpecialChestIdentifier.RAT) {
                        Vector2 cachedChestPosition = (ChestObjects[i].transform.position - new Vector3(0.1f, 0.1f));
                        IntVector2 cachedChestPositionConverted = (cachedChestPosition.ToIntVector2(VectorConversions.Floor));
                        ChestObjects[i].DeregisterChestOnMinimap();
                        Destroy(ChestObjects[i]);
                        Chest PlacedRatChestFire = targetRooms[3].SpawnRoomRewardChest(wChestObjectCollection, cachedChestPositionConverted);
                        PlacedRatChestFire.forceContentIds = cachedRatChestFire;
                    }
                    if (ChestObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[4] && ChestObjects[i].ChestIdentifier != Chest.SpecialChestIdentifier.RAT) {
                        Vector2 cachedChestPosition2 = (ChestObjects[i].transform.position - new Vector3(0.1f, 0.1f));
                        IntVector2 cachedChestPositionConverted2 = (cachedChestPosition2.ToIntVector2(VectorConversions.Floor));
                        ChestObjects[i].DeregisterChestOnMinimap();
                        Destroy(ChestObjects[i]);
                        Chest PlacedRatChestEgg = targetRooms[4].SpawnRoomRewardChest(wChestObjectCollection, cachedChestPositionConverted2);
                        PlacedRatChestEgg.forceContentIds = cachedRatChestEgg;
                    }
                }
                for (int i = 0; i < gameObjects.Length; i++) {
                    if (gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[3] | gameObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[4] && gameObjects[i].name.ToLower().StartsWith("chest")) {
                        Destroy(gameObjects[i]);
                    }
                }
            }


            AssetBundle enemiesBundle = ResourceManager.LoadAssetBundle("enemies_base_001");
            GameObject MetalGearRatPrefab = (GameObject)enemiesBundle.LoadAsset("MetalGearRat");
            AIActor MetalGearRatActorPrefab = MetalGearRatPrefab.GetComponent<AIActor>();
            MetalGearRatDeathController MetalGearRatDeathPrefab = MetalGearRatActorPrefab.GetComponent<MetalGearRatDeathController>();
            PunchoutController punchoutController = MetalGearRatDeathPrefab.PunchoutMinigamePrefab.GetComponent<PunchoutController>();


            GameObject RatNoteFire = Instantiate(punchoutController.PlayerLostNotePrefab.gameObject, RoomOffsets[3].ToVector3() + new Vector3(2, 5), Quaternion.identity);

            NoteDoer RatNoteFireNoteDoer = RatNoteFire.GetComponent<NoteDoer>();
            RatNoteFireNoteDoer.stringKey = "This chest contains the Fossilized Gun.\nUse as fire source to hatch Wierd Egg from the other chest!";
            RatNoteFireNoteDoer.useAdditionalStrings = false;
            RatNoteFireNoteDoer.alreadyLocalized = true;
            targetRooms[3].RegisterInteractable(RatNoteFireNoteDoer);

            GameObject RatNoteEgg = null;
            GameObject RatNoteElevatorHollow = null;
            GameObject RatNoteElevatorForge = null;

            if (NPCObjects.Length > 0) {
                for (int i = 0; i < NPCObjects.Length; i++) {
                    if (NPCObjects[i].transform.position.GetAbsoluteRoom() == targetRooms[4]) {
                        Vector3 cachedNPCPosition = (NPCObjects[i].transform.position - new Vector3(1.5f, 0));
                        Destroy(NPCObjects[i].gameObject);
                        RatNoteEgg = Instantiate(punchoutController.PlayerLostNotePrefab.gameObject, cachedNPCPosition, Quaternion.identity);
                        break;
                    }
                }
                if (RatNoteEgg != null) {
                    NoteDoer RatNoteEggNoteDoer = RatNoteEgg.GetComponent<NoteDoer>();
                    RatNoteEggNoteDoer.stringKey = "This chest contains the Wierd Egg.\nOpen if you plan to get Baby Dragun!";
                    RatNoteEggNoteDoer.useAdditionalStrings = false;
                    RatNoteEggNoteDoer.alreadyLocalized = true;
                    targetRooms[4].RegisterInteractable(RatNoteEggNoteDoer);
                }
            }           

            if (ExitRoomTeleporters.Length > 0) {
                for (int i = 0; i < ExitRoomTeleporters.Length; i++) {
                    if (ExitRoomTeleporters[i].transform.position.GetAbsoluteRoom() == targetRooms[0]) {
                        Vector3 cachedTeleporterPosition1 = (ExitRoomTeleporters[i].transform.position + new Vector3(4, 2));
                        RatNoteElevatorHollow = Instantiate(punchoutController.PlayerLostNotePrefab.gameObject, cachedTeleporterPosition1, Quaternion.identity);
                    }
                    if (ExitRoomTeleporters[i].transform.position.GetAbsoluteRoom() == targetRooms[2]) {
                        Vector3 cachedTeleporterPosition2 = (ExitRoomTeleporters[i].transform.position - new Vector3(2, 0) + new Vector3(0, 2));
                        RatNoteElevatorForge = Instantiate(punchoutController.PlayerLostNotePrefab.gameObject, cachedTeleporterPosition2, Quaternion.identity);
                    }

                }
            }

            if (RatNoteElevatorHollow != null) {
                NoteDoer RatNoteElevatorNoteDoerHollow = RatNoteElevatorHollow.GetComponent<NoteDoer>();
                RatNoteElevatorNoteDoerHollow.stringKey = "This elevator will take you to the Hollows.\nIf you are already on or past this floor, this elevator will take you to the next floor instead.";
                RatNoteElevatorNoteDoerHollow.useAdditionalStrings = false;
                RatNoteElevatorNoteDoerHollow.alreadyLocalized = true;
                targetRooms[0].RegisterInteractable(RatNoteElevatorNoteDoerHollow);
            }

            if (RatNoteElevatorForge != null) {
                NoteDoer RatNoteElevatorNoteDoerForge = RatNoteElevatorForge.GetComponent<NoteDoer>();
                RatNoteElevatorNoteDoerForge.stringKey = "This elevator will take you to the Forge.\nIf you are already on or past this floor, this elevator will take you to the next floor instead.";
                RatNoteElevatorNoteDoerForge.useAdditionalStrings = false;
                RatNoteElevatorNoteDoerForge.alreadyLocalized = true;
                targetRooms[2].RegisterInteractable(RatNoteElevatorNoteDoerForge);
            }
        }

        private IEnumerator StartPunchout() {
            if (PunchoutRoomCluster == null) {
                ETGModConsole.Log("ERROR: Punchout Room is null!");
                yield break;
            }

            RoomHandler CurrentRoom = GameManager.Instance.BestActivePlayer.CurrentRoom;

            AssetBundle assetBundle = ResourceManager.LoadAssetBundle("enemies_base_001");
            GameObject MetalGearRatPrefab = (GameObject)assetBundle.LoadAsset("MetalGearRat");
            AIActor MetalGearRatActorPrefab = MetalGearRatPrefab.GetComponent<AIActor>();
            MetalGearRatDeathController MetalGearRatDeathPrefab = MetalGearRatActorPrefab.GetComponent<MetalGearRatDeathController>();

            IntVector2 RoomPosition = PunchoutRoomCluster[5].area.basePosition;
            IntVector2 RoomPositionPunchout = PunchoutRoomCluster[1].area.basePosition;
            Vector3 RoomPositionVec3 = RoomPosition.ToVector3() - new Vector3(5, 5);
            Vector3 RoomPositionPunchoutVec3 = RoomPositionPunchout.ToVector3() - new Vector3(10, 10);

            GameObject RatRoomLootDropAnchor = new GameObject();
            RatRoomLootDropAnchor.name = "RatRoomLootDropAnchor";
            RatRoomLootDropAnchor.SetActive(true);

            GameObject RatRoomAnchorObject = new GameObject();
            RatRoomAnchorObject.name = "RatRoomAnchorObject";
            RatRoomAnchorObject.SetActive(false);

            GameObject RatRoomAnchorObject2 = new GameObject();
            RatRoomAnchorObject.name = "RatRoomAnchorObject2";
            RatRoomAnchorObject.SetActive(false);

            RatRoomLootDropAnchor.AddComponent<MetalGearRatRoomController>();
            RatRoomLootDropAnchor.transform.position = RoomPositionVec3;
            MetalGearRatRoomController ratRoomController = RatRoomLootDropAnchor.GetComponent<MetalGearRatRoomController>();
            ratRoomController.name = "MetalGearRatRoomControllerObject";
            ratRoomController.floorCover = RatRoomAnchorObject;
            ratRoomController.brokenMetalGear = RatRoomAnchorObject2;
            SuperReaperController.PreventShooting = true;
            foreach (PlayerController playerController in GameManager.Instance.AllPlayers) { playerController.SetInputOverride("starting punchout"); }
            yield return new WaitForSeconds(2f);
            Pixelator.Instance.FadeToColor(0.75f, Color.white, false, 0f);
            Minimap.Instance.TemporarilyPreventMinimap = true;
            yield return new WaitForSeconds(3f);
            GameObject punchoutMinigame = Instantiate(MetalGearRatDeathPrefab.PunchoutMinigamePrefab, RoomPositionPunchoutVec3, Quaternion.identity);
            PunchoutController punchoutController = punchoutMinigame.GetComponent<PunchoutController>();
            punchoutController.Init();
            while (PunchoutController.IsActive) { yield return null; }
            Minimap.Instance.TemporarilyPreventMinimap = false;
            foreach (PlayerController playerController2 in GameManager.Instance.AllPlayers) { playerController2.ClearInputOverride("starting punchout"); }
            Pixelator.Instance.FadeToColor(1f, Color.white, true, 0f);
            yield return new WaitForSeconds(1f);
            yield break;
        }
    }
}


