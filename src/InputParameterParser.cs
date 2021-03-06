﻿using Landis.Utilities;
using System.Collections.Generic;
using Landis.Core;

namespace Landis.Extension.RootRot
{
    class InputParameterParser
        : TextParser<IInputParameters>
    {
        public static ISpeciesDataset SpeciesDataset = PlugIn.ModelCore.Species;
        //---------------------------------------------------------------------
        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        //---------------------------------------------------------------------
        public InputParameterParser()
        {
        }
        //---------------------------------------------------------------------
        protected override IInputParameters Parse()
        {
            ReadLandisDataVar();

            InputParameters parameters = new InputParameters();

            InputVar<int> timestep = new InputVar<int>("Timestep");
            ReadVar(timestep);
            parameters.Timestep = timestep.Value;

            InputVar<string> inputMapName = new InputVar<string>("InputMap");
            if (ReadOptionalVar(inputMapName))
                parameters.InputMapName = inputMapName.Value;
            else
                parameters.InputMapName = null;

            //--------- Read In Species SusceptibiityTable ---------------------------------------
            Dictionary<string, int> lineNumbers = new Dictionary<string, int>();
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing SpeciesSusceptibility table.");

            const string SppParms = "SpeciesSusceptibility";
            ReadName(SppParms);
            InputVar<string> sppName = new InputVar<string>("Species");
            InputVar<float> suscept = new InputVar<float>("Susceptibility");
            InputVar<float> suscept2 = new InputVar<float>("SecondarySusceptibility");

            while ((!AtEndOfInput) && (CurrentName != "LethalTemp"))
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ReadValue(sppName, currentLine);
                ISpecies species = SpeciesDataset[sppName.Value.Actual];
                if (species == null)
                    throw new InputValueException(sppName.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                    throw new InputValueException(sppName.Value.String,
                                                  "The species {0} was previously used on line {1}",
                                                  sppName.Value.String, lineNumber);
                else
                    lineNumbers[species.Name] = LineNumber;

                parameters.SusceptibilityTable.Add(species, new float[2]);
                ReadValue(suscept, currentLine);
                parameters.SusceptibilityTable[species][0] = suscept.Value;
                ReadValue(suscept2, currentLine);
                parameters.SusceptibilityTable[species][1] = suscept2.Value;

                GetNextLine();
            }

            InputVar<float> lethalTemp = new InputVar<float>("LethalTemp");
            ReadVar(lethalTemp);
            parameters.LethalTemp = lethalTemp.Value;

            InputVar<float> minSoilTemp = new InputVar<float>("MinSoilTemp");
            ReadVar(minSoilTemp);
            parameters.MinSoilTemp = minSoilTemp.Value;

            //InputVar<float> soilTDepth = new InputVar<float>("SoilTempDepth");
            //ReadVar(soilTDepth);
            //parameters.SoilTDepth = soilTDepth.Value;
            parameters.SoilTDepth = 0.10F;  // Set to 0.10m (10cm)
            
            InputVar<float> phWet = new InputVar<float>("PhWet");
            ReadVar(phWet);
            parameters.PhWet = phWet.Value;

            InputVar<float> phDry = new InputVar<float>("PhDry");
            ReadVar(phDry);
            parameters.PhDry = phDry.Value;

            InputVar<float> phMax = new InputVar<float>("PhMax");
            ReadVar(phMax);
            parameters.PhMax = phMax.Value;

            InputVar<float> minProbID = new InputVar<float>("MinProbID");
            ReadVar(minProbID);
            parameters.MinProbID = minProbID.Value;

            InputVar<float> maxProbDI = new InputVar<float>("MaxProbDI");
            ReadVar(maxProbDI);
            parameters.MaxProbDI = maxProbDI.Value;

            InputVar<string> outputMapName = new InputVar<string>("OutputMapName");
            if (ReadOptionalVar(outputMapName))
                parameters.OutMapNamesTemplate = outputMapName.Value;
            else
                parameters.OutMapNamesTemplate = null;

            InputVar<string> toldMapName = new InputVar<string>("TOLDMapName");
            if (ReadOptionalVar(toldMapName))
                parameters.TOLDMapNamesTemplate = toldMapName.Value;
            else
                parameters.TOLDMapNamesTemplate = null;

            InputVar<string> lethalTempMapName = new InputVar<string>("LethalTempMapName");
            if (ReadOptionalVar(lethalTempMapName))
                parameters.LethalTempMapNameTemplate = lethalTempMapName.Value;
            else
                parameters.LethalTempMapNameTemplate = null;

            InputVar<string> soilTempMapName = new InputVar<string>("SoilTempMapName");
            if (ReadOptionalVar(soilTempMapName))
                parameters.SoilTempMapNameTemplate = soilTempMapName.Value;
            else
                parameters.SoilTempMapNameTemplate = null;

            InputVar<string> wetnessIndexMapName = new InputVar<string>("WetnessIndexMapName");
            if (ReadOptionalVar(wetnessIndexMapName))
                parameters.WetnessIndexMapNameTemplate = wetnessIndexMapName.Value;
            else
                parameters.WetnessIndexMapNameTemplate = null;

            InputVar<string> pSIMapName = new InputVar<string>("pUIMapName");
            if (ReadOptionalVar(pSIMapName))
                parameters.PSIMapNameTemplate = pSIMapName.Value;
            else
                parameters.PSIMapNameTemplate = null;

            InputVar<string> pIDMapName = new InputVar<string>("pIDMapName");
            if (ReadOptionalVar(pIDMapName))
                parameters.PIDMapNameTemplate = pIDMapName.Value;
            else
                parameters.PIDMapNameTemplate = null;

            InputVar<string> totalBiomassRemovedMapName = new InputVar<string>("TotalBiomassRemovedMapName");
            if (ReadOptionalVar(totalBiomassRemovedMapName))
                parameters.TotalBiomassRemovedMapNameTemplate = totalBiomassRemovedMapName.Value;
            else
                parameters.TotalBiomassRemovedMapNameTemplate = null;

            InputVar<string> speciesBiomassRemovedMapName = new InputVar<string>("SpeciesBiomassRemovedMapName");
            if (ReadOptionalVar(speciesBiomassRemovedMapName))
                parameters.SpeciesBiomassRemovedMapNamesTemplate = speciesBiomassRemovedMapName.Value;
            else
                parameters.SpeciesBiomassRemovedMapNamesTemplate = null;


            InputVar<string> eventLogFile = new InputVar<string>("EventLog");
            if (ReadOptionalVar(eventLogFile))
                parameters.EventLogFileName = eventLogFile.Value;
            else
                parameters.EventLogFileName = null;

            InputVar<string> summaryLogFile = new InputVar<string>("SummaryLog");
            if (ReadOptionalVar(summaryLogFile))
                parameters.SummaryLogFileName = summaryLogFile.Value;
            else
                parameters.SummaryLogFileName = null;

            return parameters;
        }
    }
}
