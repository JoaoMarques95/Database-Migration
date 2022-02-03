# **Exporter of Faculty and University Data**

`Enhance Universities and faculty data`

### Rules:

1. Fill "AddFaculties" or "AddUniverisites" excel.
   1. Copy the raw data from database to blue coluns.
   2. Point the Universities/Faculties you want to Update/Add, by putting the "upsert" green collum as "x".
   3. Follow the rules on the "Rules" tab.
2. Insert the json data in the exporter c# solution.
   1. Data/universities.json (Required for booth faculty exporter and univeristy exporter).
   2. Data/faculties.json (Required only for faculty exporter).
3. Check properties and genericProperties.
4. Run Script.

### Properties (Universities and Faculties Level):

1. `excelUniverisitesFile` - Insert Full Path of "AddFaculties" OR "AddFaculties". It can be anywhere.
2. `resultFilePath` - Path on witch SQL script will be displayed (.txt file).

### GenericProperties (Domain Level):

1. `apiKey` - APIKey from google cloud account. Create a project and Add **Places API** to the project.
2. `seachFields` - The fields to get in the Find Places API result.
3. `detailsFields` - The fields to get in the Places Detail API result.
4. `ValidateCandidateTypes` - Insert false, if dont want to validate the google candidates data. In this way all universities/faculties SQL will be generated without validation.
