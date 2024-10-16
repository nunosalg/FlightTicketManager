﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FlightTicketManager.Data.Entities
{
    public class Aircraft : IEntity
    {
        public int Id { get; set; }


        [Required]
        [MaxLength(50)]
        public string Model { get; set; }


        [Required]
        [MaxLength(50)]
        public string Airline { get; set; }


        [Required]
        public int Capacity { get; set; }


        [Display(Name = "Image")]
        public string ImageUrl { get; set; } 


        [Display(Name = "Active")]
        public bool IsActive { get; set; }


        public List<string> Seats { get; set; }


        public User User { get; set; }


        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(ImageUrl))
                {
                    return "~/images/noimage.png";
                }

                //return $"https://localhost:44306{ImageUrl.Substring(1)}";
                return $"http://www.flywithsalgueiro.somee.com{ImageUrl.Substring(1)}"; 
            }
        }


        public string Data => $"{Model} {Airline}";


        public string ModelId => $"{Model} {Airline} {Id}";


        /// <summary>
        /// Generates the seats of the aircraft based on its capacity.
        /// </summary>
        public void GenerateSeats()
        {
            string[] seatLetters = { "A", "B", "C", "D", "E", "F" };
            int letterIndex = 0; 
            int row = 1;
            string seat;

            List<string> generatedSeats = new List<string>(); // Initialize the seats list

            for (int i = Capacity; i > 0; i--)
            {
                seat = row < 10 ? $"0{row}{seatLetters[letterIndex]}" : $"{row}{seatLetters[letterIndex]}";
                generatedSeats.Add(seat);

                letterIndex++;

                // Reset the letter index and increment the row number when all letters are used
                if (letterIndex == seatLetters.Length)
                {
                    letterIndex = 0;
                    row++;
                }
            }

            Seats = generatedSeats;
        }

        /// <summary>
        /// Updates de seats according to the new capacity
        /// </summary>
        /// <param name="newCapacity"></param>
        public void UpdateCapacity(int newCapacity)
        {
            Capacity = newCapacity;
            GenerateSeats(); 
        }
    }
}
