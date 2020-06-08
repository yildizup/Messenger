CREATE DATABASE telefonico;

create table users (
 
  email varchar(255),
  fsname varchar(255), 
  password varchar(255),
  status boolean, 
  
  primary key (email)
  
  );
  
  create table contacts (
	
	main_email varchar(255),
	friend_email varchar(255),
	

 foreign key (main_email)
	references users(email),
 foreign key (friend_email)
	references users(email)
  
  );
  
  
  create table chat (
  
  main_email varchar(255),
    friend_email varchar(255), 
  message text,
  thetime timestamp,
  received boolean,
    

 foreign key (main_email)
    references users(email),
 foreign key (friend_email)
    references users(email)
  );
  
  
 -- bulk insert 
 -- Die Benutzer
 
insert into users (fsname, email, password, status) values 
('Thomas Anderson','neo@matrix.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Joseph A. Cooper','cooper@interstellar.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Elicia Coates','elicia.coates@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Daniaal Merrill','daniaal.merrill@outlook.de','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Kean Bannister','b.kean@live.de','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Ammara Medrano','a.medrano@gmx.de','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Elisha Harmon','e.harmon@telefonico.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Kyra Cooper','k.cooper@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Johnnie Senior','j.senior@mail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
-- Datensätze mit leichten email Adressen
('Der Admin','admin@telefonico.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Ein Benutzer','user@telefonico.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Der Absender','absender@gmail.com','356a192b7913b04c54574d18c28d46e6395428ab',0),
('Der Empfänger','empfänger@gmail.com','356a192b7913b04c54574d18c28d46e6395428ab',0),
('Ein Freund','freund1@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Ein Freund2','freund2@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Ein Freund3','freund3@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0),
('Ein Freund4','freund4@gmail.com','7110eda4d09e062aa5e4a390b0a572ac0d2c0220',0);

-- Die Kontakte der Benutzer

-- Kontakte von admin@telefonico.com
insert into contacts (main_email, friend_email) values 
('admin@telefonico.com','user@telefonico.com'),
('admin@telefonico.com','absender@gmail.com'),
('admin@telefonico.com','empfänger@gmail.com'),
('admin@telefonico.com','freund1@gmail.com'),
('admin@telefonico.com','freund2@gmail.com'),
('admin@telefonico.com','freund3@gmail.com'),
('admin@telefonico.com','freund4@gmail.com'),
-- Kontakte von absender@gmail.com
('absender@gmail.com','empfänger@gmail.com'),
('absender@gmail.com','freund1@gmail.com'),
('absender@gmail.com','neo@matrix.com'),
('absender@gmail.com','e.harmon@telefonico.com'),
-- Kontakte von empfänger@gmail.com
('empfänger@gmail.com','absender@gmail.com'),
('empfänger@gmail.com','neo@matrix.com'),
-- Kontakte von neo@matrix.com
('neo@matrix.com','cooper@interstellar.com'),
('neo@matrix.com','e.harmon@telefonico.com'),
('neo@matrix.com','daniaal.merrill@outlook.de'),
('neo@matrix.com','a.medrano@gmx.de'),
-- Kontakt von cooper@interstellar.com
('cooper@interstellar.com','neo@matrix.com'),
('cooper@interstellar.com','daniaal.merrill@outlook.de'),
('cooper@interstellar.com','a.medrano@gmx.de'),
('cooper@interstellar.com','e.harmon@telefonico.com');

-- Der Chat

-- zwischen absender@gmail.com und empfänger@gmail.com
insert into chat (main_email, friend_email, message, thetime, received) values 
('absender@gmail.com','empfänger@gmail.com','Hallo Empfänger !', '2020-02-15 08:46:38', true),
('empfänger@gmail.com','absender@gmail.com','Hallo Absender. Wie geht es dir ?', '2020-02-15 08:48:00', true),
('absender@gmail.com','empfänger@gmail.com','Danke, mir geht es gut und dir Empfänger ?', '2020-02-15 08:49:38', true),
('empfänger@gmail.com','absender@gmail.com','Herzlichen Dank für die Nachfrage Absender. Mir geht es auch gut.', '2020-02-15 08:55:38', true),
('absender@gmail.com','empfänger@gmail.com','Das Freut mich Empfänger.', '2020-02-15 09:10:12', true),
-- zwischen admin@telefonico.com und user@telefonico.com
('user@telefonico.com','admin@telefonico.com','Sehr geehrter Herr admin, ich brauche Ihre Hilfe.', '2020-02-15 03:10:12', true),
('admin@telefonico.com','user@telefonico.com','Hallo Kunde, wie kann ich Ihnen weiterhelfen ?', '2020-02-15 03:12:11', true),
('user@telefonico.com','admin@telefonico.com','Hat sich geklärt. Dennoch vielen Dank für die schnelle Antwort.', '2020-02-15 05:10:12', true),
-- zwischen neo@matrix.com und cooper@interstellar.com
('neo@matrix.com','cooper@interstellar.com','Hallo Cooper. Wo bist du gerade ?', '2020-02-15 08:46:38', true),
('cooper@interstellar.com','neo@matrix.com','Hey Neo. Ich "fliege" gerade durch ein Wurmloch. Sry, für die späte Antwort. Und was machst du ?', '2020-02-18 08:48:00', true),
('neo@matrix.com','cooper@interstellar.com','Kein Problem. Ach nicht viel. Wir sind gerade in einer Besprechung.', '2020-02-19 08:49:38', true),
('cooper@interstellar.com','neo@matrix.com','Ok, viel Erfolg.', '2020-02-19 08:55:38', true),
('neo@matrix.com','cooper@interstellar.com','Danke dir auch.', '2020-02-19 09:10:12', true);
