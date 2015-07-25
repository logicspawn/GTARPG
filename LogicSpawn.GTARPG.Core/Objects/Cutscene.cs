using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Math;

namespace LogicSpawn.GTARPG.Core.Objects
{
    public class Cutscene
    {
        private List<CutsceneAction> _actions;
        private List<CutsceneAction> _startActions;
        private List<CutsceneAction> _endActions;

        private Dictionary<string,object> _cutsceneObjects;

        public string CutsceneID;
        public bool Running;

        public Cutscene(string name)
        {
            CutsceneID = name;
            _actions = new List<CutsceneAction>();
            _startActions = new List<CutsceneAction>();
            _endActions = new List<CutsceneAction>();
        }

        public List<Entity> NeededEntities
        {
            get { var e = new List<Entity>();
                var actions = _actions.Concat(_startActions).Concat(_endActions);
                foreach (var cutsceneAction in actions)
                {
                    object ped;
                    if(cutsceneAction.Params.TryGetValue("Ped", out ped))
                    {
                        e.Add((Entity)ped);
                    }

                    object veh;
                    if(cutsceneAction.Params.TryGetValue("Vehicle", out veh))
                    {
                        e.Add((Entity)veh);
                    }
                }

                return e;
            }
        }

        public Cutscene OnStart(params CutsceneAction[] actions)
        {
            _startActions = actions.ToList();
            return this;
        }
        public Cutscene OnEnd(params CutsceneAction[] actions)
        {
            _endActions = actions.ToList();
            return this;
        }

        public Cutscene AddObject(string name, object obj)
        {
            _cutsceneObjects.Add(name, obj);
            return this;
        }
        public Cutscene AddCamera(string name, Vector3 position, Vector3 rotation)
        {

            var c = World.CreateCamera(position, rotation, GameplayCamera.FieldOfView);
            _cutsceneObjects.Add(name, c);
            return this;
        }

        public Cutscene AddAction(CutsceneAction action)
        {
            _actions.Add(action);
            return this;
        }

        private T GetObject<T>(string name)
        {
            return _cutsceneObjects[name] is T ? (T)_cutsceneObjects[name] : default(T);
        }

        public void Run()
        {
            int curAction = 0;
            while (curAction < _actions.Count)
            {
                DoAction(_actions[curAction]);
                curAction++;
                Script.Wait(0);
            }
        }

        public void OnStart()
        {
            int curAction = 0;
            while(curAction < _startActions.Count)
            {
                DoAction(_startActions[curAction]);
                curAction++;
                Script.Wait(0);
            }
        }

        private void DoAction(CutsceneAction a)
        {
            switch(a.Type)
            {
                case ActionType.Wait:
                {
                    var time = a.Param<int>("Time");
                    Script.Wait(time);
                    break;
                }
                case ActionType.SetCamera:
                {
                    var camName = a.Param<string>("Camera");
                    World.RenderingCamera = GetObject<Camera>(camName);
                    break;
                }
                case ActionType.MoveCamera:
                {
                    var camName = a.Param<string>("Camera");
                    var cam = GetObject<Camera>(camName);


                    var pos = a.Param<Vector3>("Position");
                    var rot = a.Param<Vector3>("Rotation");

                    cam.Position = pos;
                    cam.Rotation = rot;
                    break;
                }
                case ActionType.MoveCameraRel:
                {
                    var camName1 = a.Param<string>("Camera");
                    var cam = GetObject<Camera>(camName1);


                    var ped = a.Param<Ped>("Ped");
                    var pos = a.Param<Vector3>("Position");
                    var rot = a.Param<Vector3>("Rotation");

                    cam.Position = ped.Position + pos;
                    cam.Rotation = ped.Rotation + rot;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnEnd()
        {
            int curAction = 0;
            while (curAction < _endActions.Count)
            {
                DoAction(_endActions[curAction]);
                curAction++;
                Script.Wait(0);
            }
        }
    }

    public class CutsceneAction
    {
        public ActionType Type;
        public Dictionary<string, object> Params;

        public CutsceneAction(ActionType type, Dictionary<string, object> parameters)
        {
            Type = type;
            Params = parameters;
        }

        public static CutsceneAction Wait(int time)
        {
            var parameters = new Dictionary<string, object>
                                 {
                                     {"Time", time}
                                 };
            return new CutsceneAction(ActionType.Wait, parameters);
        }

        public static CutsceneAction SetCamera(string camera)
        {
            var parameters = new Dictionary<string, object>
                                 {
                                     {"Camera", camera}
                                 };
            return new CutsceneAction(ActionType.SetCamera, parameters);   
        }
        public static CutsceneAction MoveCamera(string camera, Vector3 position, Vector3 rotation)
        {
            var parameters = new Dictionary<string, object>
                                 {
                                     {"Camera", camera},
                                     {"Position", position},
                                     {"Rotation", rotation},
                                 };
            return new CutsceneAction(ActionType.MoveCamera, parameters);   
        }

        public static CutsceneAction MoveCameraRel(string camera, Ped ped, Vector3 position, Vector3 rotation)
        {
            var parameters = new Dictionary<string, object>
                                 {
                                     {"Camera", camera},
                                     {"Ped", ped},
                                     {"Position", position},
                                     {"Rotation", rotation},
                                 };
            return new CutsceneAction(ActionType.MoveCameraRel, parameters);      
        }

        public T Param<T>(string s)
        {
            return Params[s] is T ? (T) Params[s] : default(T);
        }
    }

    public enum ActionType
    {
        Wait,
        SetCamera,
        MoveCamera,
        MoveCameraRel,
    }
}