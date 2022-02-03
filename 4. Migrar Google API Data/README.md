# **GoogleAPIExporter**

`Enhance Universities and faculty data with google API Places`

### Rules:

1. Check the properties.
2. Insert the DB data in json, in "data" directory.
3. Assure that all universities are in the "data". If the faculty universitiy is not stored, the faculty will be ignored by the script.

### Porperties:

1. `apiKey` - APIKey from google cloud account. Create a project and Add **Places API** to the project.
2. `seachFields` - The fields to get in the Find Places API result.
3. `detailsFields` - The fields to get in the Places Detail API result.
4. `ValidateCandidateTypes` - Insert false, if dont want to validate the google candidates data. In this way all universities/faculties SQL will be generated without validation.
5. `updateGoogleDataOnlyIfDoesNotExist` - Set true, if you want to update only the univeristies/faculties that does not have the google data already.
6. `FilterUniveristyCountryCode` - Insert "all" if you want to run for all universities. Intert contry codes separated by "," that you want to filter.
7. `resultFilePath` - Path on witch SQL script will be displayed (.txt file).
