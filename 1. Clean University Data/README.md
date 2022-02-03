# 1. Export universites and UEE raw data in excel format

Put it in download raw data folder

# 2. Clean data

1. Copy university raw data to "2. Clean Data\Universities.xlsx"
2. Generate "Fuzzy LookUp" data.
3. Fill cases fields in excel.

   - **Actualizar universidade** → Update university name.
   - **Actualizar faculdade** → Update Faculdade name (Faculty name is everything after "-")
   - **Actualizar flag** → Update flag deleted (F / V)
   - **Eliminar universidade** → ` If "x"` → Eliminar Universidade dados inválidos Elimina também a user Education Experience. É verificado no início e elimina tudo.
   - **Eliminar faculdades** → `If "x"` → Eliminar faculdades dados inválidos. Mas a universidade é válida. Elimina apenas o nome da faculdade, é apenas verificada no final, para o caso de a entrada necessitar de actualizar o nome da universidade / flag
   - **Not Listed Institution** → `If "x"` → Delete university and put the university name as a "notListedInstitution" in UEE table.
   - **Mudar API List Name** → `If "x"` → Change API list entry to the new/actual university name.

# 3. Run SQL in DB.

## Prepare c# Data on "generator" c# program

1.  Copy university data with cases fields in excel to "3. Prepare c# Script Data\UniversitiesSelected.xlsx".
2.  Export booth "UniversitiesSelected.xlsx" and "UEE.xlsx" to csv (separated by ";") to "4. Run c# Script\Generator\Data".

## Run c# program

1.  Run Script
2.  NOTE: The script separates the univerity and faculties by "|". If there are some error with the script check the "Properties" class static fields.

## Run sql in BD.

2.  Run the contents of in the database SQL scripts "4. Run c# Script\Generator\Result".
