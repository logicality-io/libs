using System;

namespace Logicality.EventSourcing.Domain;

public delegate Type MessageTypeResolver(string name);