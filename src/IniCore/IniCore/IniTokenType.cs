namespace IniCore;

public enum IniTokenType
{
    /// <summary>
    /// Error occurred.
    /// </summary>
    Error,
    /// <summary>
    /// End of file; lexing ends here.
    /// </summary>
    Eof,
    /// <summary>
    /// if a line contains a # outside a string literal, it and all characters after it in the line are considered a comment and ignored.
    /// </summary>
    Comment,
    /// <summary>
    /// The line feed character \n. which is used to determine comments' ownership.
    /// </summary>
    LineFeed,
    /// <summary>
    /// A specific setting or configuration option that is defined within a section. 
    /// </summary>
    Key,
    /// <summary>
    /// The data or information associated with a key.
    /// </summary>
    Value,
    /// <summary>
    /// Represents the name of a top-level section in the INI file.<br/>
    /// This token type is used for the first part of the section header.
    /// </summary>
    SectionName,
    /// <summary>
    /// Represents the name of a subsection within a nested section in the INI file.<br/>
    /// This token type is used for parts of the section header that come after the first dot (.) in a nested structure.
    /// </summary>
    SubsectionName,
}