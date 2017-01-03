package com.perkinelmer.cboe.registration.repository;

import com.perkinelmer.cboe.registration.model.User;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.rest.core.annotation.RepositoryRestResource;
import org.springframework.data.repository.query.Param;

import java.util.List;

public interface UserRepository extends MongoRepository<User,String>{
    List<User> findByUsername(@Param("username") String username);
}