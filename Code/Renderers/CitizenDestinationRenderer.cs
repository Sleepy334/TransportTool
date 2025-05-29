using ColossalFramework;
using SleepyCommon;
using System;
using System.Collections.Generic;
using UnityEngine;
using static PublicTransportInfo.MainPanel;
using static RenderManager;

namespace PublicTransportInfo
{
    public class CitizenDestinationRenderer : SimulationManagerBase<CitizenDestinationRenderer, MonoBehaviour>, IRenderableManager
    {
        public CitizenDestinationRenderer()
        {
            SimulationManager.RegisterManager(instance);
        }

        protected override void BeginOverlayImpl(CameraInfo cameraInfo)
        {
            base.BeginOverlayImpl(cameraInfo);

            if (MainPanel.IsVisible() && ModSettings.GetSettings().HighlightCitizenDestination)
            {
                StopInfo info = MainPanel.Instance.GetStopInfo();
                if (info.m_currentStopId != 0)
                {
                    Building[] BuildingBuffer = BuildingManager.instance.m_buildings.m_buffer;
                    NetNode[] NodeBuffer = NetManager.instance.m_nodes.m_buffer;

                    HashSet<InstanceID> instances = EnumerateWaitingPassengers(cameraInfo, info.m_currentStopId);
                    foreach (InstanceID instance in instances)
                    {
                        if (instance.Building != 0)
                        {
                            RendererUtils.HighlightBuilding(BuildingBuffer, instance.Building, cameraInfo, Color.magenta);
                        }
                        else if (instance.NetNode != 0)
                        {
                            RendererUtils.HighlightNode(cameraInfo, NodeBuffer[instance.NetNode], Color.magenta);
                        }
                        else
                        {
                            CDebug.Log($"Type: {instance.Type} Index: {instance.Index}");
                        }
                    }
                }
            }
        }

        private HashSet<InstanceID> EnumerateWaitingPassengers(CameraInfo cameraInfo, ushort stop)
        {
            HashSet<InstanceID> instances = new HashSet<InstanceID>();
            if (stop == 0)
            {
                return instances;
            }

            ushort nextStop = TransportLine.GetNextStop(stop);
            if (nextStop == 0)
            {
                return instances;
            }

            
            CitizenManager instance = Singleton<CitizenManager>.instance;
            NetManager instance2 = Singleton<NetManager>.instance;

            Vector3 position = instance2.m_nodes.m_buffer[stop].m_position;
            Vector3 position2 = instance2.m_nodes.m_buffer[nextStop].m_position;

            float searchDistance = 32f;
            int num2 = Mathf.Max((int)((position.x - searchDistance) / 8f + 1080f), 0);
            int num3 = Mathf.Max((int)((position.z - searchDistance) / 8f + 1080f), 0);
            int num4 = Mathf.Min((int)((position.x + searchDistance) / 8f + 1080f), 2159);
            int num5 = Mathf.Min((int)((position.z + searchDistance) / 8f + 1080f), 2159);

            for (int i = num3; i <= num5; i++)
            {
                for (int j = num2; j <= num4; j++)
                {
                    ushort cimId = instance.m_citizenGrid[i * 2160 + j];
                    int num8 = 0;
                    while (cimId != 0)
                    {
                        CitizenInstance citizenInstance = CitizenManager.instance.m_instances.m_buffer[cimId];
                        ushort nextGridInstance = citizenInstance.m_nextGridInstance;
                        if ((citizenInstance.m_flags & CitizenInstance.Flags.WaitingTransport) != 0)
                        {
                            Vector3 a = citizenInstance.m_targetPos;
                            if (Vector3.SqrMagnitude(a - position) < searchDistance * searchDistance)
                            {
                                CitizenInstance cimInstance = instance.m_instances.m_buffer[cimId];
                                CitizenInfo info2 = cimInstance.Info;
                                if (cimInstance.Info.m_citizenAI.TransportArriveAtSource(cimId, ref cimInstance, position, position2))
                                {
                                    // Found a waiting passenger
                                    InstanceID target = cimInstance.Info.m_citizenAI.GetTargetID(cimId, ref cimInstance);
                                    if (target.Index != 0)
                                    {
                                        instances.Add(target);
                                        
                                    }
                                }
                            }
                        }

                        cimId = nextGridInstance;
                        if (++num8 > 65536)
                        {
                            CDebug.Log("Invalid list detected!\n" + Environment.StackTrace);
                            break;
                        }
                    }
                }
            }

            return instances;
        }
    }
}
