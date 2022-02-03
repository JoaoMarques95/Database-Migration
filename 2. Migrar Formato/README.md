# **Stript1**

`Is used to insert the faculties in faculty table`

### Steps:

1. Export universities information in json from database
2. Run Script
3. Run SQL in DB.

### Rules:

- **Only unique faculties of the same university are inserted**
- **The faculty name is everything after the "|"(check the Helper class Static Fields)**
- **The flag "Deleted" is propagated**
- **An auxiliary collum is created "orginalUniversityId" is created for matching purposes in the step 2-- This collum is to delete in the end**
- **Only the first university that apears with the same name, is mantained!**

# **Stript2**

`Is used to insert facultyId in userExperience. Update universitiesId's in userExperience. Delete repeted universities and finally delete faculty name from universities names`

### Steps:

1. Export universities, faculties and user Experience information in json from database
2. Run Script
3. Run SQL in DB. Run only 1 block at a time:

   - Update UniversityId's in User experience
   - Update facultiesId's in user experience
   - Delete universities
   - Update Universities names
