# Persist
A seamless way to allow components in your Unity scenes to be uniquely and safely identifiable, and persistable between application runs.

All you need to do is make your component implement `IPersistableScript`, then use the functions in `PersistUtility` to save/load the components in your scenes to a serializable format.