using System.Collections.Generic;
using ColossalFramework;


namespace MoreCitizenUnits
{
	/// <summary>
	/// Utilities class for dealing with CitizenUnits.
	/// </summary>
	internal static class CitizenUtils
	{
		// Counter.
		private static uint citizenCount = 0;

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
			uint bufferLength = (uint)originalCitizens.m_buffer.Length;
			Logging.Message("creating new Citizen array with length ", bufferLength);
			citizenManager.m_citizens = new Array32<Citizen>(bufferLength);
			Logging.Message("finished creating array");

			// Array buffer references.
			CitizenUnit[] citizenUnitBuffer = citizenManager.m_units.m_buffer;
			Citizen[] newCitizenBuffer = citizenManager.m_citizens.m_buffer;
			Citizen[] oldCitizenBuffer = originalCitizens.m_buffer;

			// Iterate through each CitizenUnit and copy each citizen referred to.
			for (uint i = 0; i < citizenUnitBuffer.Length; ++i)
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

			Logging.Message("copied ", citizenCount, " citizens");
		}


		/// <summary>
		/// Tries to copy a Citizen from the given old buffer to the given new buffer.
		/// </summary>
		/// <param name="CopyCitizen">Citizen ID to copy</param>
		/// <param name="oldUnitBuffer">Old Citizen buffer to copy from</param>
		/// <param name="newUnitBuffer">New Citizen buffer to copy into</param>
		/// <returns>True if copying was succesful, false otherwise (if no empty unit in the chain was found)</returns>
		private static void CopyCitizen(uint citizenID, Citizen[] oldBuffer, Citizen[] newBuffer)
		{
			// Bounds check.
			if (citizenID > newBuffer.Length)
			{
				Logging.Error("invalid citizen id ", citizenID);
				return;
			}

			// If this citizen hasn't already been copied, it's a valid target; copy it over.
			if ((newBuffer[citizenID].m_flags & Citizen.Flags.Created) != 0)
			{
				newBuffer[citizenID] = oldBuffer[citizenID];
				Logging.Message("copied citizen ", citizenID);

				// Increment counter.
				++citizenCount;
			}
		}
	}
}
