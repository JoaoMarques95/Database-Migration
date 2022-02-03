# Correct solution

## Front-end API.

1. Implement in front-end side the API that list all universities.

## SQL to correct university data.

1. When data is using the API correctly, do scripts in excel to update the information (SQL):
   - Update Universities name (acording to universities List)
   - Delete invalid universities/UUE associated. (Flag True) (by hand)
   - Remove invalid faculty names. (flag true) (by hand)
   - If there is only university name then change the flag deleted to "false".

## .c# program to split the data acordingly to the new DB stack

The idea is to implement this "flag" aproach in the UEE level. To then FE list certain faculties for that university.

`steps`

1.  Separate information into:
    - `Dictionary <Guid1, String1, List< guid, String2, int >>`
2.  Dictionary where we will have one university for multiple faculties.
    - `Guid1` - University 1º id -- university that will remain.
    - `List<Guid2>` - List of original University Id's for that Faculty
    - `String1` - University Name
    - `String2` - Faculty Name
    - `int` - Flag that is at university level
      NOTE: If university does not have any faculty, add a faculty with the same name as the university.
3.  Do some clean up in the faculty data.
    - If university have more than 2 faculties.
    - Compare faculty names using the following algorithm, if faculty matches with other use following algorithm: https://github.com/kdjones/fuzzystring
    - If faculty names are similar, delete one of the faculties but mantain the original universityId in the `List<Guid2>`.
4.  LOOP Per each faculty entry:
    - Insert BD faculty (name, universityId, flag)
    - Get the created faculty Id
    - In the UEE table, where universityId=`List<Guid>` change the value booth values of "facultyId" (from the id instanciated) and "notListedInstitution" (from the flag).
5.  Delete all universities entries that does not contain the id in `Guid1`.
6.  Update all universities names that have as id the `Guid1` with the `String1`.

//se um universidade não tiver uma faculdade associada, o Id daquela universidade n pode ser perdido no ponto 4.
