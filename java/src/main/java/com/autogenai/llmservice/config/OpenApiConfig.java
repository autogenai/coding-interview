package com.autogenai.llmservice.config;

import io.swagger.v3.oas.models.ExternalDocumentation;
import io.swagger.v3.oas.models.OpenAPI;
import io.swagger.v3.oas.models.info.Contact;
import io.swagger.v3.oas.models.info.Info;
import io.swagger.v3.oas.models.info.License;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;

@Configuration
public class OpenApiConfig {

    @Bean
    public OpenAPI llmServiceOpenAPI() {
        return new OpenAPI()
                .info(new Info()
                        .title("LLM Chat Service API")
                        .description("API documentation for the simple streaming chat endpoint.")
                        .version("v0.0.1")
                        .contact(new Contact().name("AutoGenAI").email("support@example.com"))
                        .license(new License().name("Apache 2.0").url("https://www.apache.org/licenses/LICENSE-2.0"))
                )
                .externalDocs(new ExternalDocumentation()
                        .description("Project Help")
                        .url("/HELP.md")
                );
    }
}
