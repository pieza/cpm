namespace cpm
{
    static class DefaultFiles
    {
        public static string CPMSettings = """"
        {
          "RepositoryUrl": "http://165.227.207.127:5001/api",
          "CachePath": "cache"
        }
        """";

        public static string HelloWorld = """
        #include <stdio.h>

        int main() {
            printf("Hello World!\n");
            return 0;
        }
        """;

        public static string Gitignore = """
        .include/
        build/
        """;


        public static string BuildMakefile(string projectName)
        {
            return $"""
            CC = gcc
            CFLAGS = -Wall -I./.include
            BUILD_DIR = build

            {projectName}: $(BUILD_DIR)/main.o
            {"\t"}$(CC) $(CFLAGS) -o $(BUILD_DIR)/{projectName} $(BUILD_DIR)/main.o

            $(BUILD_DIR)/main.o: main.c | $(BUILD_DIR)
            {"\t"}$(CC) $(CFLAGS) -c main.c -o $(BUILD_DIR)/main.o

            run: {projectName}
            {"\t"}./$(BUILD_DIR)/{projectName}

            $(BUILD_DIR):
            {"\t"}mkdir -p $(BUILD_DIR)

            clean:
            {"\t"}rm -rf $(BUILD_DIR)
            """;

        }
    }
}
