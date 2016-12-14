package com.perkinelmer.cboe.registration.repository;

import com.perkinelmer.cboe.registration.model.Project;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.rest.core.annotation.RepositoryRestResource;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface ProjectRepository extends MongoRepository<Project,String>{
    List<Project> findByName(@Param("name") String name);
}