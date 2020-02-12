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
  
  
 -- Die Benutzer
insert into user (email, password) values ('absender@gmail.com','1');
insert into user (email, password) values ('empfänger@gmail.com','1');
insert into user (email, password) values ('nocheinfreund@gmail.com','1');


-- Die Kontakte
insert into contacts (main_email, friend_email) values ('absender@gmail.com','empfänger@gmail.com');
insert into contacts (main_email, friend_email) values ('absender@gmail.com','nocheinfreund@gmail.com');