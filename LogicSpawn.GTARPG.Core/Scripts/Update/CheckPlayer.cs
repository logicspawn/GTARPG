using System;
using GTA;
using GTA.Math;
using GTA.Native;
using LogicSpawn.GTARPG.Core.General;
using LogicSpawn.GTARPG.Core.Scripts.Popups;

namespace LogicSpawn.GTARPG.Core
{
    public class CheckPlayer : UpdateScript
    {
        protected override bool RunWhenGameIsNotLoaded { get { return false; } }
        protected override bool RunWhenPlayerDead { get { return true; } }

        public override void Update()
        {
            Ped player = Game.Player.Character;

            if (player.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player.Handle))
            {
                RPG.PlayerDead = true;
                Wait(3000);
                var deathPosition = Game.Player.Character.Position;
                try
                {
                    RPG.UIHandler.CloseAll();
                    RPG.UIHandler.ShowUI = false;
                    var c = World.CreateCamera(Game.Player.Character.Position + new Vector3(0, 0, 100), new Vector3(0, 0, 90), GameplayCamera.FieldOfView);
                    World.RenderingCamera = c;

                    var oldModel = RPG.PlayerData.ModelHash;

                    Model m = PedHash.Michael;
                    m.Request(1000);
                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, m.Hash);
                    Function.Call(Hash.SET_PED_DEFAULT_COMPONENT_VARIATION, Game.Player.Character.Handle);
                    m.MarkAsNoLongerNeeded();

                    while (Game.Player.Character.IsDead || Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player.Handle))
                    {
                        Wait(0);
                    }

                    var t = 150;
                    while(t > 0)
                    {
                        UI.ShowSubtitle("Loading",100);
                        Function.Call(Hash.DISPLAY_HUD, 0);
                        Function.Call(Hash.DISPLAY_RADAR, 0);
                        World.RenderingCamera = c;
                        Wait(100);
                        t--;
                    }

                    //Wait(15000);
                    Model oldM = oldModel;
                    oldM.Request(1000);
                    Function.Call(Hash.SET_PLAYER_MODEL, Game.Player.Handle, oldM.Hash);
                    Wait(0);
                    Game.Player.Character.FreezePosition = false;

                    RPGMethods.OnRespawn();
                    RPG.UIHandler.ShowUI = true;
                    Function.Call(Hash.DISPLAY_HUD, 1);
                    Function.Call(Hash.DISPLAY_RADAR, 1);
                    World.RenderingCamera = null;
                    c.Destroy();
                    Game.FadeScreenIn(500);
                    RPG.PlayerDead = false;
                    Wait(15000);
                }
                catch(Exception ex)
                {
                    RPGLog.Log(ex.ToString());
                }
            }
        }

    }
}