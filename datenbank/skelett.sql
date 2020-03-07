CREATE DATABASE telefonico;

create table user (
 
  email varchar(255),
  password varchar(255),
  
  primary key (email)
  
  );
  
  create table contacts (
	
	main_email varchar(255),
	friend_email varchar(255),
	

 foreign key (main_email)
	references user(email),
 foreign key (friend_email)
	references user(email)
  
  ) ;
  
  
  
  
  create table chat (
  
  main_email varchar(255),
    friend_email varchar(255), 
  message text,
  thetime timestamp,
  received boolean,
    

 foreign key (main_email)
    references user(email),
 foreign key (friend_email)
    references user(email)
  ) ;
  
  
  
  
 -- Die Benutzer
insert into user (email, password) values ('admin@telefonico.com','1');
insert into user (email, password) values ('user@telefonico.com','1');
insert into user (email, password) values ('absender@gmail.com','1');
insert into user (email, password) values ('empfänger@gmail.com','1');
insert into user (email, password) values ('freund1@gmail.com','1');
insert into user (email, password) values ('freund2@gmail.com','1');
insert into user (email, password) values ('freund3@gmail.com','1');
insert into user (email, password) values ('freund4@gmail.com','1');


-- Die Kontakte

-- Kontakte von admin@telefonico.com
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','user@telefonico.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','absender@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','empfänger@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund1@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund2@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund3@gmail.com');
insert into contacts (main_email, friend_email) values ('admin@telefonico.com','freund4@gmail.com');

-- Kontakte von absender@gmail.com
insert into contacts (main_email, friend_email) values ('absender@gmail.com','empfänger@gmail.com');
insert into contacts (main_email, friend_email) values ('absender@gmail.com','freund1@gmail.com');

-- Kontakte von empfänger@gmail.com

insert into contacts (main_email, friend_email) values ('empfänger@gmail.com','absender@gmail.com');



-- Der Chat

-- zwischen absender@gmail.com und empfänger@gmail.com
insert into chat (main_email, friend_email, message, thetime, received) values ('absender@gmail.com','empfänger@gmail.com','Hallo empfänger', '2020-02-15 08:46:38', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('empfänger@gmail.com','absender@gmail.com','Hallo Absender. Wie geht es dir ?', '2020-02-15 08:48:00', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('absender@gmail.com','empfänger@gmail.com','Danke, mir geht es gut und dir Empfänger ?', '2020-02-15 08:49:38', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('empfänger@gmail.com','absender@gmail.com','Herzlichen Dank für die Nachfrage. Mir geht es auch gut', '2020-02-15 08:55:38', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('absender@gmail.com','empfänger@gmail.com','Das Freut mich Empfänger mein Freund', '2020-02-15 09:10:12', true);

-- zwischen admin@telefonico.com und user@telefonico.com
insert into chat (main_email, friend_email, message, thetime, received) values ('user@telefonico.com','admin@telefonico.com','Sehr geehrter Herr admin, \n ich brauche Ihre Hilfe.', '2020-02-15 03:10:12', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('admin@telefonico.com','user@telefonico.com','Hallo Kunde, \n wie kann ich Ihnen weiterhelfen ?', '2020-02-15 03:12:11', true);
insert into chat (main_email, friend_email, message, thetime, received) values ('user@telefonico.com','admin@telefonico.com','Hat sich geklärt. Dennoch vielen Dank für die schnelle Antwort.', '2020-02-15 05:10:12', true);

-- zwischen absender@gmail.com und freund1@gmail.com
insert into chat (main_email, friend_email, message, thetime, received) values ('absender@gmail.com','freund1@gmail.com','Hallo Freund1. Wie geht es dir ?', '2020-02-16 19:18:11', true);