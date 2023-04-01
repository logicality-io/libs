---
title: WebhookRelay Client
---
stateDiagram-v2
    [*] --> Disconnected
    Disconnected --> Connecting : [T] Connect
    Connecting --> Connecting: Transient error, retry indefinitely (log warnings)
    Connecting --> ConnectingFailed: Non-transient-error.
    Connecting --> Disposed: DisposeAsync
    Connecting --> Connected
    Connected --> ConnectionLost
    ConnectionLost --> Connecting: Reconnect
    Connected --> Authenticating
    Authenticating --> Authenticated
    Authenticating --> AuthenticationFailed
    AuthenticationFailed --> [*]
    Authenticating --> ConnectionLost
    Authenticated --> HandlingMessages
    HandlingMessages --> ConnectionLost
    Connected --> Disposed : DisposeAsync
    Authenticating --> Disposed : DisposeAsync
    Authenticated --> Disposed : DisposeAsync
    HandlingMessages --> Disposed : DisposeAsync
    ConnectingFailed --> [*]
    Disposed --> [*]