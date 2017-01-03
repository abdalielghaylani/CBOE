package com.perkinelmer.cboe.registration.authenticate;
import static org.springframework.hateoas.mvc.ControllerLinkBuilder.*;

import com.fasterxml.jackson.core.JsonFactory;
import com.fasterxml.jackson.core.JsonGenerator;
import com.fasterxml.jackson.databind.JsonNode;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.node.ObjectNode;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;
import com.fasterxml.jackson.databind.ObjectMapper;

import java.io.IOException;

@RestController
public class Controller {
    @RequestMapping(path="/api/auth/login", method= RequestMethod.POST)
    public HttpEntity<JsonNode> greeting(
            @RequestBody(required = true) JsonNode user) throws IOException {
        ObjectMapper m = new ObjectMapper();
        JsonNode result = m.readTree("{ \"data\": { \"msg\": \"LOGIN SUCCESSFUL\" }, " +
            "\"meta\": { \"id\": 1, \"token\": \"abcd1234\", \"expires\": \"2020-01-01\", \"profile\": { \"fullName\": \"Admin User\" }}}");
        return new ResponseEntity<JsonNode>(result, HttpStatus.OK);
    }
}