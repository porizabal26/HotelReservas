-- Crear la base de datos HOTEL
CREATE DATABASE HOTEL;
GO

-- Usar la base de datos HOTEL
USE HOTEL;
GO

-- CREACION DE TABLAS
CREATE TABLE Clientes (
	ClienteID INT PRIMARY KEY IDENTITY(1,1),
	Nombre VARCHAR(100),
	Email VARCHAR(50)
);

CREATE TABLE Habitaciones (
	HabitacionID INT PRIMARY KEY IDENTITY(1,1),
	TipoHabitacion VARCHAR(50),
	PrecioPorNoche DECIMAL(10,2)
);

CREATE TABLE Reservas (
	ReservaID INT PRIMARY KEY IDENTITY(1,1),
	ClienteID INT FOREIGN KEY REFERENCES Clientes(ClienteID),
	HabitacionID INT FOREIGN KEY REFERENCES Habitaciones(HabitacionID),
	FechaEntrada DATETIME,
	FechaSalida DATETIME,
	Total DECIMAL(10,2)
);
GO

-- CONSULTAS

-- Obtener el total de ingresos generados por cliente
SELECT C.Nombre, SUM(R.Total) AS TotalIngresos
FROM Reservas R
JOIN Clientes C ON R.ClienteID = C.ClienteID
GROUP BY C.Nombre;

-- Obtener las reservas activas (donde la fecha de salida es posterior a la fecha actual)
SELECT R.ReservaID, C.Nombre, H.TipoHabitacion, R.FechaEntrada, R.FechaSalida
FROM Reservas R
JOIN Clientes C ON R.ClienteID = C.ClienteID
JOIN Habitaciones H ON R.HabitacionID = H.HabitacionID
WHERE R.FechaSalida > GETDATE();
GO

-- Crear un procedimiento almacenado para registrar una nueva reserva
CREATE PROCEDURE RegistrarReserva
    @ClienteID INT,
    @HabitacionID INT,
    @FechaEntrada DATETIME,
    @FechaSalida DATETIME
AS
BEGIN
    DECLARE @NumeroNoches INT;
    DECLARE @PrecioPorNoche DECIMAL(10, 2);
    DECLARE @Total DECIMAL(10, 2);

    SET @NumeroNoches = DATEDIFF(DAY, @FechaEntrada, @FechaSalida);
    
    SELECT @PrecioPorNoche = PrecioPorNoche
    FROM Habitaciones
    WHERE HabitacionID = @HabitacionID;

    SET @Total = @NumeroNoches * @PrecioPorNoche;

    INSERT INTO Reservas (ClienteID, HabitacionID, FechaEntrada, FechaSalida, Total)
    VALUES (@ClienteID, @HabitacionID, @FechaEntrada, @FechaSalida, @Total);
END;
GO