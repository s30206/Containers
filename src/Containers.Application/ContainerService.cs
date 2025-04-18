﻿using Containers.Models;
using Microsoft.Data.SqlClient;

namespace Containers.Application;

public class ContainerService : IContainerService
{
    
    private string _connectionString;
    public ContainerService(string connectionString)
    {
        this._connectionString = connectionString;
    }
    
    public IEnumerable<Container> GetAllContainers()
    {
        List<Container> containers = [];
        const string query = "Select * from containers";

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(query, connection);
            
            connection.Open();
            
            SqlDataReader reader = command.ExecuteReader();

            try
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var container = new Container()
                        {
                            ID = reader.GetInt32(0),
                            ContainerTypeId = reader.GetInt32(1),
                            IsHazardious = reader.GetBoolean(2),
                            Name = reader.GetString(3),
                        };
                        
                        containers.Add(container);
                    }
                }
            }
            finally
            {
                reader.Close();
            }
        }

        return containers;
    }

    public bool CreateContainer(Container container)
    {
        const string insertString =
            "insert into containers (ContainerTypeId, IsHazardious, Name ) values (@ContainerTypeId, @IsHazardious, @Name)";

        int countRows = -1;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            SqlCommand command = new SqlCommand(insertString, connection);
            command.Parameters.AddWithValue("@ContainerTypeId", container.ContainerTypeId);
            command.Parameters.AddWithValue("@IsHazardious", container.IsHazardious);
            command.Parameters.AddWithValue("@Name", container.Name);
            
            connection.Open();
            
            countRows = command.ExecuteNonQuery();
        }
        
        return countRows != -1;
    }
}