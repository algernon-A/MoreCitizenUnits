using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony transpilers to replace hardcoded CitizenUnit limits in the game.
    /// </summary>
    [HarmonyPatch]
    public static class GameLimitTranspiler
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, identified methods with hardcoded CitizenUnit limits.
        /// This includes CitizenManager.Awake where the CitizenUnits array is created; overriding the value here automatically creates arrays of the correct new size.
        /// </summary>
        /// <returns>List of target methods to patch</returns>
        public static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(CitizenManager), "Awake");
            yield return AccessTools.Method(typeof(BuildingAI), "EnsureCitizenUnits");
            yield return AccessTools.Method(typeof(CampusBuildingAI), nameof(CampusBuildingAI.HandleDead2));
            yield return AccessTools.Method(typeof(CemeteryAI), nameof(CemeteryAI.GetDebugString));
            yield return AccessTools.Method(typeof(CemeteryAI), "GetDeadCount");
            yield return AccessTools.Method(typeof(CemeteryAI), "ProduceGoods");
            yield return AccessTools.Method(typeof(CemeteryAI), nameof(CemeteryAI.GetLocalizedStats));
            yield return AccessTools.Method(typeof(ChildcareAI), nameof(ChildcareAI.GetDebugString));
            yield return AccessTools.Method(typeof(ChildcareAI), nameof(ChildcareAI.ReleaseBuilding));
            yield return AccessTools.Method(typeof(CommonBuildingAI), nameof(CommonBuildingAI.GetDebugString));
            yield return AccessTools.Method(typeof(CommonBuildingAI), "EmptyBuilding");
            yield return AccessTools.Method(typeof(CommonBuildingAI), "GetHomeBehaviour");
            yield return AccessTools.Method(typeof(CommonBuildingAI), "GetWorkBehaviour");
            yield return AccessTools.Method(typeof(CommonBuildingAI), "GetStudentBehaviour");
            yield return AccessTools.Method(typeof(CommonBuildingAI), "GetVisitBehaviour");
            yield return AccessTools.Method(typeof(EldercareAI), nameof(EldercareAI.ReleaseBuilding));
            yield return AccessTools.Method(typeof(HospitalAI), nameof(HospitalAI.GetDebugString));
            yield return AccessTools.Method(typeof(HospitalAI), nameof(HospitalAI.ReleaseBuilding));
            yield return AccessTools.Method(typeof(HospitalAI), "ProduceGoods");
            yield return AccessTools.Method(typeof(HospitalAI), nameof(HospitalAI.GetLocalizedStats));
            yield return AccessTools.Method(typeof(IndustryBuildingAI), "HandleDead2");
            yield return AccessTools.Method(typeof(MuseumAI), nameof(MuseumAI.HandleDead2));
            yield return AccessTools.Method(typeof(ParkBuildingAI), nameof(ParkBuildingAI.CountVisitors));
            yield return AccessTools.Method(typeof(ParkBuildingAI), "HandleDead2");
            yield return AccessTools.Method(typeof(PoliceStationAI), "ProduceGoods");
            yield return AccessTools.Method(typeof(PoliceStationAI), nameof(PoliceStationAI.GetLocalizedStats));
            yield return AccessTools.Method(typeof(ResidentialBuildingAI), "GetAverageResidentRequirement");
            yield return AccessTools.Method(typeof(ShelterAI), "HandleWorkAndVisitPlaces");
            yield return AccessTools.Method(typeof(ShelterAI), nameof(ShelterAI.GetLocalizedStats));
            yield return AccessTools.Method(typeof(TourBuildingAI), "CountUsers", new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(TransportPassengerData).MakeByRefType() });
            yield return AccessTools.Method(typeof(Building), nameof(Building.GetEmptyCitizenUnit));
            yield return AccessTools.Method(typeof(Building), nameof(Building.GetNotFullCitizenUnit));
            yield return AccessTools.Method(typeof(Building), nameof(Building.FindCitizenUnit));
            yield return AccessTools.Method(typeof(BuildingManager.Data), nameof(BuildingManager.Data.AfterDeserialize));
            // ResidentAI.SimulationStep (any of 3) has no checks
            // ResidentAI.UnitHasChild has no checks
            yield return AccessTools.Method(typeof(ResidentAI), "FinishSchoolOrWork");
            // ResidentAI.StartTransfer has no checks
            yield return AccessTools.Method(typeof(ResidentAI), "TryJoinVehicle");
            yield return AccessTools.Method(typeof(TouristAI), "TryJoinVehicle");
            // Citizen.SetHome has no checks
            // Citizen.SetWorkplace has no checks
            // Citizen.SetStudentplace has no checks
            // Citizen.SetVisitplace has no checks
            // Citizen.SetVehicle has no checks
            yield return AccessTools.Method(typeof(Citizen), nameof(Citizen.GetContainingUnit));
            yield return AccessTools.Method(typeof(Citizen), nameof(Citizen.AddToUnits));
            yield return AccessTools.Method(typeof(Citizen), nameof(Citizen.RemoveFromUnits));
            // CizenManager.Awake - arrays are initialized here
            // CitizenManager.CreateUnits has no checks
            yield return AccessTools.Method(typeof(CitizenManager), nameof(CitizenManager.ReleaseUnits));
            // CitizenManager.ReleaseUnitImplementation has no checks
            // CitizenManager.SimulationStepImpl - simulation framing is here (128 units per step)
            // CitizenManager.Data.Serialize TODO
            // CitizenManager.Data.Deserialize is patched in CitizenDeserialze
            // CitizenManager.Data.AfterDeserialize has no checks
            yield return AccessTools.Method(typeof(DisasterHelpers), nameof(DisasterHelpers.RemovePeople));
            yield return AccessTools.Method(typeof(DisasterHelpers), nameof(DisasterHelpers.SavePeople));
            yield return AccessTools.Method(typeof(EventAI), nameof(EventAI.CountVisitors));
            yield return AccessTools.Method(typeof(CityServiceWorldInfoPanel), "UpdateWorkers");
            yield return AccessTools.Method(typeof(IndustryWorldInfoPanel), "UpdateWorkersAndTotalUpkeep");
            yield return AccessTools.Method(typeof(WarehouseWorldInfoPanel), "UpdateWorkers");
            yield return AccessTools.Method(typeof(ZonedBuildingWorldInfoPanel), "UpdateWorkers");
            yield return AccessTools.Method(typeof(ZonedBuildingWorldInfoPanel), "UpdateResidential");
            yield return AccessTools.Method(typeof(MessageManager), "GetRandomCitizenID", new Type[] { typeof(uint), typeof(CitizenUnit.Flags) });
            yield return AccessTools.Method(typeof(AmbulanceAI), nameof(AmbulanceAI.GetBufferStatus));
            yield return AccessTools.Method(typeof(AmbulanceAI), "ArriveAtTarget");
            yield return AccessTools.Method(typeof(AmbulanceAI), nameof(AmbulanceAI.CanLeave));
            yield return AccessTools.Method(typeof(AmbulanceAI), "ArriveAtSource");
            yield return AccessTools.Method(typeof(AmbulanceCopterAI), "GetPatientCitizen");
            yield return AccessTools.Method(typeof(AmbulanceCopterAI), nameof(AmbulanceCopterAI.CanLeave));
            // BicycleAI.SimulationStep (any of 2) has no checks
            yield return AccessTools.Method(typeof(BicycleAI), "GetDriverInstance");
            yield return AccessTools.Method(typeof(BusAI), nameof(BusAI.TransportArriveAtTarget));
            yield return AccessTools.Method(typeof(DisasterResponseCopterAI), nameof(DisasterResponseCopterAI.CanLeave));
            yield return AccessTools.Method(typeof(DisasterResponseVehicleAI), nameof(DisasterResponseVehicleAI.CanLeave));
            yield return AccessTools.Method(typeof(FireTruckAI), nameof(FireTruckAI.CanLeave));
            yield return AccessTools.Method(typeof(HearseAI), nameof(HearseAI.GetBufferStatus));
            yield return AccessTools.Method(typeof(HearseAI), nameof(HearseAI.ReleaseVehicle));
            yield return AccessTools.Method(typeof(HearseAI), nameof(HearseAI.CanLeave));
            yield return AccessTools.Method(typeof(HearseAI), "LoadDeadCitizens");
            yield return AccessTools.Method(typeof(HearseAI), "ArriveAtSource");
            yield return AccessTools.Method(typeof(ParkMaintenanceVehicleAI), nameof(ParkMaintenanceVehicleAI.CanLeave));
            yield return AccessTools.Method(typeof(PassengerCarAI), "UnloadPassengers");
            yield return AccessTools.Method(typeof(PassengerCarAI), "GetDriverInstance");
            yield return AccessTools.Method(typeof(PassengerCarAI), "ParkVehicle");
            yield return AccessTools.Method(typeof(PoliceCarAI), nameof(PoliceCarAI.CountCriminals));
            yield return AccessTools.Method(typeof(PoliceCarAI), "ArrestCriminals");
            yield return AccessTools.Method(typeof(PoliceCarAI), nameof(PoliceCarAI.CanLeave));
            yield return AccessTools.Method(typeof(PoliceCarAI), "UnloadCriminals");
            yield return AccessTools.Method(typeof(PoliceCopterAI), nameof(PoliceCopterAI.CountCriminals));
            yield return AccessTools.Method(typeof(TaxiAI), "GetPassengerInstance");
            yield return AccessTools.Method(typeof(TaxiAI), "UnloadPassengers");
            yield return AccessTools.Method(typeof(TaxiAI), "ParkVehicle");
            yield return AccessTools.Method(typeof(TrolleybusAI), nameof(TrolleybusAI.TransportArriveAtTarget));
            yield return AccessTools.Method(typeof(VehicleAI), "EnsureCitizenUnits");
            yield return AccessTools.Method(typeof(VehicleAI), nameof(VehicleAI.CanLeave));
            // Vehicle.GetTargetFrame has no checks
            yield return AccessTools.Method(typeof(Vehicle), nameof(Vehicle.GetNotFullCitizenUnit));
            yield return AccessTools.Method(typeof(VehicleManager.Data), nameof(VehicleManager.Data.AfterDeserialize));
            // CitizenManager.SimulationStepImpl is patched in SimulationStepImplPatch.
            // CityServiceWorldInfoPanel.UpdateWorkers
        }


        /// <summary>
        /// Harmony transpiler to replace hardcoded CitizenUnit limits.
        /// Finds ldc.i4 524288 (which is unique in game code to the CitizenUnit limit checks) and replaces the operand with our updated maximum.
        /// </summary>
        /// <param name="original">Original (target) method</param>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Patched ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(MethodBase original, IEnumerable<CodeInstruction> instructions)
        {
            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            // Status flag.
            bool foundTarget = false;

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction and add it to output.
                instruction = instructionsEnumerator.Current;

                // Is this ldc.i4 524288 (CitizenManger.ReleaseUnits)?
                if (instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is int thisInt && thisInt == 524288)
                {
                    // Yes - change operand to our new unit count max.
                    //instruction.operand = (int)CitizenDeserialze.NewUnitCount;

                    // Set flag.
                    foundTarget = true;
                }

                // Output instruction.
                yield return instruction;
            }

            // If we got here without finding our target, something went wrong.
            if (!foundTarget)
            {
                Logging.Error("no ldc.i4 524288 found for ", original);
            }
        }
    }
}