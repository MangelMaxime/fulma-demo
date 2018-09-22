module Data.Forum

open System

type Author =
    { Id : int
      Firstname : string
      Surname : string
      Avatar : string }

type Question =
    { Id : int
      Author : Author
      Title : string
      Description : string
      CreatedAt : DateTime }

type Answer =
    { Id : int
      CreatedAt : DateTime
      Author : Author
      Content : string
      Score : int }
