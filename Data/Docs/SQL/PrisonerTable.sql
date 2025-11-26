CREATE TABLE Prisoners (
    PrisonerId INT IDENTITY(1,1) PRIMARY KEY,
    PrisonerNumber NVARCHAR(50) NULL,
    NationalId NVARCHAR(50) NOT NULL UNIQUE,
    FullName NVARCHAR(150) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    BirthDate DATE NOT NULL,
    Nationality NVARCHAR(100) NOT NULL,
    LegalStatus NVARCHAR(50) NOT NULL, -- موقوف / محكوم / مفرج عنه
    EntryDate DATE NOT NULL,
    ReleaseDate DATE NULL,
    CrimeDescription NVARCHAR(MAX) NULL,
    CurrentBlock NVARCHAR(100) NULL,
    CurrentRoom NVARCHAR(50) NULL,
    CreatedBy NVARCHAR(100) NULL,
    Notes NVARCHAR(MAX) NULL,
    AttachmentsPath NVARCHAR(260) NULL,
    IsArchived BIT NOT NULL DEFAULT(0)
);
