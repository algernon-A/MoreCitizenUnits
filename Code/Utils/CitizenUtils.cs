// <copyright file="CitizenUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System.Collections.Generic;
    using AlgernonCommons;
    using ColossalFramework;

    /// <summary>
    /// Utilities class for dealing with CitizenUnits.
    /// </summary>
    internal static class CitizenUtils
    {
        // Hashset of allocated citizen IDs.
        private static HashSet<uint> allocatedCitizens;

        /// <summary>
        /// Copies CitizenUnits from a deserialized array to a new clean array.
        /// </summary>
        internal static void ResetCitizens()
        {
            // Manager references.
            CitizenManager citizenManager = Singleton<CitizenManager>.instance;

            // Store original units.
            Array32<Citizen> originalCitizens = Singleton<CitizenManager>.instance.m_citizens;
            if (originalCitizens == null || originalCitizens.m_buffer.Length == 0)
            {
                Logging.Error("no original citizens reference set; aborting copy");
                return;
            }

            // Reset array.
            int bufferLength = originalCitizens.m_buffer.Length;
            Logging.Message("creating new Citizen array with length ", bufferLength);
            citizenManager.m_citizens = new Array32<Citizen>((uint)bufferLength);
            Logging.Message("finished creating array");

            // Array buffer references.
            CitizenUnit[] citizenUnitBuffer = citizenManager.m_units.m_buffer;
            CitizenInstance[] citizenInstanceBuffer = citizenManager.m_instances.m_buffer;
            Citizen[] newCitizenBuffer = citizenManager.m_citizens.m_buffer;
            Citizen[] oldCitizenBuffer = originalCitizens.m_buffer;

            // Initialize allocated citizens hashset.
            allocatedCitizens = new HashSet<uint>();

            // Allocate *all* citizens in new array; we'll release unused ones later.
            // Need to do it this way as we need to retain original Citizen IDs, and there's no mechanism to force a specific ID.
            bool allocating;
            int allocated = 0;
            do
            {
                allocating = citizenManager.m_citizens.CreateItem(out uint _);
                ++allocated;
            }
            while (allocating);

            Logging.Message("initally allocated ", allocated, " citizens");

            // Iterate through each CitizenUnit and copy each citizen referred to.
            for (uint i = 1; i < citizenUnitBuffer.Length; ++i)
            {
                CitizenUnit thisUnit = citizenUnitBuffer[i];

                if (thisUnit.m_citizen0 != 0)
                {
                    CopyCitizen(thisUnit.m_citizen0, oldCitizenBuffer, newCitizenBuffer);
                }

                if (thisUnit.m_citizen1 != 0)
                {
                    CopyCitizen(thisUnit.m_citizen1, oldCitizenBuffer, newCitizenBuffer);
                }

                if (thisUnit.m_citizen2 != 0)
                {
                    CopyCitizen(thisUnit.m_citizen2, oldCitizenBuffer, newCitizenBuffer);
                }

                if (thisUnit.m_citizen3 != 0)
                {
                    CopyCitizen(thisUnit.m_citizen3, oldCitizenBuffer, newCitizenBuffer);
                }

                if (thisUnit.m_citizen4 != 0)
                {
                    CopyCitizen(thisUnit.m_citizen4, oldCitizenBuffer, newCitizenBuffer);
                }
            }

            // Iterate through each Citizen Instance and copy each citizen referred to.
            for (uint i = 1; i < citizenInstanceBuffer.Length; ++i)
            {
                if (citizenInstanceBuffer[i].m_flags != CitizenInstance.Flags.None)
                {
                    CopyCitizen(citizenInstanceBuffer[i].m_citizen, oldCitizenBuffer, newCitizenBuffer);
                }
            }

            Logging.Message("copied ", allocatedCitizens.Count, " citizens");

            // Now, step through the array and release all citizens NOT included in our allocated array.
            for (uint i = 1; i < bufferLength; ++i)
            {
                if (!allocatedCitizens.Contains(i))
                {
                    try
                    {
                        citizenManager.m_citizens.ReleaseItem(i);
                    }
                    catch
                    {
                        Logging.Error("exception releasing citizen ", i);
                    }
                }
            }

            // Finally, restore original citizen array.
            Logging.Message("restoring original citizen buffer");
            newCitizenBuffer.CopyTo(oldCitizenBuffer, 0);
            citizenManager.m_citizens.m_buffer = oldCitizenBuffer;
        }

        /// <summary>
        /// Tries to copy a Citizen from the given old buffer to the given new buffer.
        /// </summary>
        /// <param name="citizenID">Citizen ID to copy.</param>
        /// <param name="oldBuffer">Old Citizen buffer to copy from.</param>
        /// <param name="newBuffer">New Citizen buffer to copy into.</param>
        private static void CopyCitizen(uint citizenID, Citizen[] oldBuffer, Citizen[] newBuffer)
        {
            // Don't copy null citizens.
            if (citizenID == 0)
            {
                return;
            }

            // Bounds check.
            if (citizenID > newBuffer.Length)
            {
                Logging.Error("invalid citizen id ", citizenID);
                return;
            }

            // If this citizen hasn't already been copied, it's a valid target; copy it over.
            if (allocatedCitizens.Add(citizenID))
            {
                newBuffer[citizenID] = oldBuffer[citizenID];
            }
        }
    }
}
