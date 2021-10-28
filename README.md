[![openupm](https://img.shields.io/npm/v/com.beardphantom.persist?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.beardphantom.persist/)

# Persist
A seamless way to allow components in your Unity scenes to be uniquely and safely identifiable, and persistable between application runs.

All you need to do is make your component implement `IPersistableScript`, then use the functions in `PersistUtility` to save/load the components in your scenes to a serializable format.
