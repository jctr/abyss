#[macro_export]
/// Creates a CommandRegistration based on a dynamic command signature.
///
/// See the README for information about how to use this macro.
macro_rules! command {
    (
        $([  $($cmd_attr_name:ident=$cmd_attr_value:expr)*  ])? // Attributes - [attr=value, attr=value]
        $cmd_name: literal, // Name
        $description: literal, // Description
        $( { $($precondition_name:expr$(,)? )* }$(,)? )? // Preconditions
        $( // Parameters
            $([
                $( { $($param_precondition_name:expr$(,)? )* }$(,)? )? // Parameter client preconditions
                $($param_attr_name:ident=$param_attr_value:expr)*  // Parameter attributes
            ])?
            $param_name:ident: $param_type:ty, // Parameter name and type
        )*
        @$exec_target_fn: ident // Execution target
    ) => {
        {
            use commands_lib::execution::{CommandContext,CommandOutput};
            use commands_lib::make_command::make_command;
            use commands_lib::command::CommandParameter;
            use commands_lib::registry::CommandMap;
            use std::collections::HashMap;

            // This ignores when cmd_attrs is empty (no attributes)
            #[allow(unused_mut)]

            // Compile command-level attributes
            let mut cmd_attrs = HashMap::new();
            $(
                $(
                    cmd_attrs.insert(stringify!($cmd_attr_name), $cmd_attr_value.to_string()); // buh?
                )*
            )?

            // Compile parameters
            let mut params = Vec::<CommandParameter>::new();
            $(
                // Parameter-level attributes
                let mut attrs = HashMap::new();
                $(
                    $(
                        attrs.insert(stringify!($param_attr_name), $param_attr_value.to_string());
                    )*
                )?
                params.push(CommandParameter {
                    rust_type: stringify!($param_type),
                    attrs: attrs,
                    name: stringify!($param_name)
                });
            )*

            fn invoke (ctx: &CommandContext) -> CommandOutput {
                $exec_target_fn(ctx, $(ctx.map.get::<$param_type>(stringify!($param_name)),)*)
            }
            make_command($cmd_name, $description, cmd_attrs, params, invoke)
        }
    }
}
pub use command;