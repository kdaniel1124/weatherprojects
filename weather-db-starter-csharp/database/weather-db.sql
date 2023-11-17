USE master;
GO

DROP DATABASE IF EXISTS [weather-db];
GO

CREATE DATABASE [weather-db];
GO

USE [weather-db];
GO

DROP TABLE IF EXISTS weather, users;

CREATE TABLE users (
  id INT IDENTITY PRIMARY KEY,
  username VARCHAR(255) NOT NULL UNIQUE, -- Username
  password VARCHAR(48) NOT NULL, -- Password (hashed, not plain-text)
  salt VARCHAR(256) NOT NULL -- Password Salt
);

CREATE TABLE weather (
    id INT IDENTITY PRIMARY KEY,
    user_id int NOT NULL,
    zipcode int NOT NULL,
    main varchar(200) NOT NULL,
    description varchar (300) NOT NULL,
    temperature numeric(5, 2),
    CONSTRAINT fk_users FOREIGN KEY (user_id) REFERENCES users (id)
);

SELECT * FROM users
SELECT * FROM weather

SELECT user_id, zipcode, main, description, temperature 
FROM weather 
WHERE user_id = @user_id

GO
