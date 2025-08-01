# 🏰 Caminos de la Fe: Cruzada y Conquista ⚔️

## 🎮 **Descripción del Proyecto**

**Caminos de la Fe: Cruzada y Conquista** es un RPG 3D multijugador medieval desarrollado en Unity que implementa el modelo Games Lab de OpenNexus. El juego combina mecánicas RPG tradicionales con un sofisticado sistema de monetización diseñado para generar revenue sostenible mientras ofrece una experiencia premium a los jugadores.

### 🎯 **Concepto Principal**
Un RPG medieval donde los jugadores eligen entre tres facciones históricas:
- **🔵 Cruzados** - Los guerreros de la fe cristiana
- **🟢 Sarracenos** - Los defensores del Islam
- **🟣 Antiguos** - Los guardianes de la sabiduría ancestral

---

## 🚀 **Características Principales**

### ⚔️ **Sistema de Combate**
- Combate en tiempo real con mecánicas de rango y cooldown
- Sistema de auto-combate con cálculos matemáticos avanzados
- Modificadores por facción y estadísticas del jugador
- IA enemiga con 4 estados: Idle, Chasing, Attacking, Returning

### 📊 **Progresión del Jugador**
- **8 estadísticas principales**: Vida, Maná, Ataque, Defensa, Velocidad, Crítico, Resistencia, Suerte
- Sistema de experiencia con nivelado automático
- Mejora de stats con costo cuadrático (anti-inflación)
- 3 facciones con bonificaciones únicas

### 🎒 **Sistema de Items**
- **5 niveles de rareza**: Common, Uncommon, Rare, Epic, Legendary
- **Sistema de equipamiento completo**: Armas, armaduras, accesorios
- **Inventario avanzado**: 45 slots con sistema de stacks
- **Ítems legendarios únicos** por servidor con mecánicas especiales
- **Sistema de monturas**: 6 tipos con bonificaciones específicas

### 🎯 **Sistema de Monetización Games Lab**
- **Faith Pass**: Battle Pass de $10/mes con beneficios premium
- **Economía USD-Gold escalable**: Level 1-10 ($1=1000g), Level 11+ ($1=100×Level)
- **Sistema de publicidad**: Anuncios cada 15min + rewarded videos
- **Límites diarios**: Anti-farming con compensación F2P
- **Faith Tokens**: Moneda de evento para jugadores gratuitos

---

## 🛠️ **Tecnologías Utilizadas**

- **Unity 2025.x** - Motor de juego principal
- **C#** - Lenguaje de programación
- **Unity Input System** - Manejo de controles
- **JSON** - Serialización de datos
- **PlayerPrefs** - Persistencia local
- **Event-Driven Architecture** - Comunicación entre sistemas

### 📦 **Arquitectura de Namespaces**
```
CaminosDeLaFe/
├── Core/           # Sistemas principales
├── Data/           # Configuración y datos
├── Entities/       # Player, Enemy, NPCs
├── UI/             # Interfaces de usuario
├── Items/          # Sistema de ítems
├── Inventory/      # Gestión de inventario
├── Systems/        # Monturas, combate, etc.
└── Monetization/   # Sistemas de revenue
```

---

## 🎮 **Cómo Jugar**

### **Instalación**
1. Clona este repositorio
2. Abre el proyecto en Unity 2025.x o superior
3. Carga la escena principal
4. Presiona Play

### **Controles**
- **WASD** - Movimiento del personaje
- **Clic derecho** - Atacar enemigos
- **F10** - Probar sistema de monturas
- **F11** - Recibir ítems de prueba
- **F12** - Ejecutar tests de sistemas

### **Primeros Pasos**
1. Selecciona tu facción preferida
2. Explora el mundo medieval
3. Combate enemigos para ganar XP y oro
4. Mejora tus estadísticas
5. Colecciona equipamiento y monturas

---

## 💰 **Modelo de Monetización**

### **Para Jugadores F2P**
- Juego completamente gratuito con límites diarios
- Faith Tokens como compensación al alcanzar límites
- Anuncios opcionales para bonificaciones
- Progresión clara hacia contenido premium

### **Faith Pass ($10/mes)**
- 6 meses de experiencia sin anuncios
- +50% experiencia y +30% oro
- +3 misiones diarias adicionales
- Acceso a ítems y monturas exclusivas
- 50 niveles de recompensas premium

### **Compras de Oro**
- Level 1-10: $1 = 1000 oro
- Level 11+: $1 = 100 × tu nivel en oro
- Paquetes con bonificaciones crecientes
- Sistema anti-inflación integrado

---

## 🏗️ **Estado de Desarrollo**

### ✅ **Completado (95%)**
- ✅ Sistemas core de juego
- ✅ Sistema de combate y IA
- ✅ Progresión completa de jugador
- ✅ Sistema de ítems y equipamiento
- ✅ Sistema de monturas
- ✅ Todos los sistemas de monetización
- ✅ UI completa y funcional
- ✅ Herramientas de testing y debug

### 🔧 **En Desarrollo (5%)**
- 🔧 Referencias de assembly Unity
- 🔧 Integración final de sistemas
- 🔧 Modelos 3D y efectos visuales
- 🔧 Sistema de audio

### 📋 **Próximas Características**
- Quest system avanzado
- Multiplayer y faction wars
- NPCs y sistema de diálogos
- Construcción de fortalezas
- Integración blockchain (NFTs)

---

## 🧪 **Testing y Debug**

### **Herramientas de Debug Incluidas**
- `GameSystemTester` - Tests automáticos de todos los sistemas
- `AutoSceneSetup` - Configuración automática de escenas
- `AssemblyReferences` - Helper para referencias Unity
- Controles F10-F12 para testing rápido

### **Cómo Probar los Sistemas**
1. Ejecuta el juego en Unity
2. Presiona F12 para tests automáticos
3. Revisa la consola para resultados
4. Usa F11 para ítems de prueba
5. Usa F10 para probar monturas

---

## 🌐 **Integración OpenNexus**

Este juego forma parte del ecosistema **OpenNexus** y está diseñado para integrarse con:

### **Productos Hermanos**
- **CounterCoreHazardAV** - Antivirus empresarial
- **CajaCentralPOS** - Sistema punto de venta
- **Otros Games Lab** - Portfolio de juegos

### **Beneficios Cross-Platform**
- +20% recompensas por usar otros productos
- Sistema de loyalty points compartido
- Descuentos y promociones cruzadas
- Analytics unificado

---

## 💡 **Para Desarrolladores**

### **Estructura del Código**
- **Event-Driven Architecture** para escalabilidad
- **Namespace organization** para mantenibilidad
- **SOLID principles** aplicados consistentemente
- **Extensive documentation** en español e inglés

### **Sistemas Principales**
1. **GameManager** - Control central del juego
2. **Player** - Entidad principal del jugador
3. **MonetizationManager** - Hub de sistemas de revenue
4. **FaithPassSystem** - Battle Pass implementation
5. **EconomySystem** - USD-Gold economy

### **Patrones de Diseño Utilizados**
- **Singleton** para managers principales
- **Observer** para eventos de sistema
- **Strategy** para diferentes tipos de ítems
- **Factory** para creación de entidades
- **Command** para acciones del jugador

---

## 📈 **Métricas de Éxito**

### **KPIs Objetivo**
- **DAU (Daily Active Users)**: 10,000+
- **Faith Pass Conversion**: 5-15%
- **ARPU (Average Revenue Per User)**: $3-8/mes
- **Retention Day 7**: 35%+
- **Ad Completion Rate**: 85%+

### **Revenue Streams**
1. **Faith Pass subscriptions** ($10/mes)
2. **Gold purchases** (escalable por nivel)
3. **Advertising revenue** (banners + interstitials)
4. **Premium item sales**
5. **Cross-ecosystem bonuses**

---

## 🤝 **Contribuciones**

Este proyecto es parte del ecosistema OpenNexus. Para contribuir:

1. Fork el repositorio
2. Crea una branch para tu feature
3. Implementa los cambios con tests
4. Submite un Pull Request
5. Sigue las convenciones de código establecidas

### **Convenciones de Código**
- **C# naming conventions** estándar
- **XML documentation** para métodos públicos  
- **Unit tests** para lógica crítica
- **Event-driven communication** entre sistemas

---

## 📞 **Soporte y Contacto**

- **Desarrollador Principal**: OpenNexus Games Lab Team
- **Versión**: 1.0.0-beta
- **Unity Version**: 2025.x LTS
- **Última actualización**: Agosto 2025

### **Links Importantes**
- [Documentación Técnica](./IMPLEMENTATION_STATUS.md)
- [Sistema de Monetización](./MONETIZATION_COMPLETE.md)
- [Guía de Desarrollo](./PROYECTO_COMPLETADO.md)

---

## 📜 **Licencia**

Este proyecto es propiedad de **OpenNexus** y forma parte de la estrategia Games Lab como motor de adquisición y financiamiento del ecosistema de productos.

**Copyright © 2025 OpenNexus. Todos los derechos reservados.**

---

## 🎖️ **Reconocimientos**

- **Unity Technologies** - Motor de juego
- **OpenNexus Team** - Concepto y estrategia
- **Games Lab Model** - Framework de monetización
- **Medieval History Consultants** - Autenticidad histórica

---

*🏰 "En los Caminos de la Fe, cada decisión forja el destino de tu facción" ⚔️*

**¡Únete a la cruzada y conquista tu lugar en la historia!**
│   ├── Core/
│   │   └── GameManager.cs          # Main game controller
│   ├── Data/
│   │   ├── GameConfig.cs           # Game configuration constants
│   │   └── Factions.cs             # Faction definitions and data
│   ├── Entities/
│   │   ├── Player.cs               # Player controller and logic
│   │   └── Enemy.cs                # Enemy AI and behavior
│   ├── Systems/
│   │   ├── PlayerStats.cs          # Player statistics system
│   │   └── AutoCombatSystem.cs     # Combat calculations
│   ├── UI/
│   │   ├── FactionSelectionUI.cs   # Faction selection screen
│   │   ├── GameUI.cs               # Main game UI controller
│   │   ├── HealthManaBar.cs        # Health and mana bar components
│   │   └── PlayerStatsUI.cs        # Stats upgrade interface
│   └── SceneSetup.cs               # Scene initialization helper
```

## Key Features

### ✅ Implemented
- **Faction System**: Three factions (Cruzados, Sarracenos, Antiguos) with unique colors and properties
- **Player Stats System**: 8 upgradeable attributes with quadratic cost scaling
- **Combat System**: Real-time combat with attack ranges and damage calculations
- **Enemy AI**: Basic AI with states (Idle, Chasing, Attacking, Returning)
- **UI Systems**: 
  - Faction selection screen
  - Health and mana bars
  - Stats upgrade interface
  - Gold and level display
- **Auto Combat**: Battle outcome calculations with faction modifiers
- **Experience & Leveling**: XP gain from combat and automatic level progression

### 🚧 To Be Implemented
- **Inventory System**: Equipment slots and item management
- **Quest System**: Mission progression and rewards
- **Faction Wars**: Multiplayer faction competition
- **Equipment System**: Weapons, armor, and stat bonuses
- **Mount System**: Horse bonuses and mobility
- **Save/Load System**: Game progress persistence

## Setup Instructions

### 1. Scene Setup
1. Open the `SampleScene` in Unity
2. Create an empty GameObject and name it "SceneSetup"
3. Attach the `SceneSetup` script to it
4. The script will automatically create necessary components when you play the scene

### 2. Manual Setup (Alternative)
1. Create an empty GameObject named "GameManager"
2. Attach the `GameManager` script to it
3. Create an EventSystem for UI (GameObject > UI > Event System)

### 3. Controls
- **WASD**: Move player
- **Left Mouse**: Attack
- **Tab**: Toggle stats UI
- **ESC**: Pause (to be implemented)

### 4. Testing
1. Press Play in Unity
2. Select a faction from the UI
3. Move around and fight enemies
4. Press Tab to upgrade stats with earned gold

## Code Architecture

### Core Systems
- **GameManager**: Singleton pattern managing game states and scene transitions
- **PlayerStats**: Event-driven stat system with automatic UI updates
- **AutoCombatSystem**: Mathematical combat calculations with faction modifiers

### UI Architecture
- Modular UI components with automatic layout
- Event-driven updates for real-time data display
- Scalable design for different screen resolutions

### Entity System
- Component-based architecture following Unity best practices
- Separate concerns: movement, combat, AI, stats
- Easy to extend with new entity types

## Migration Notes

### From Python/Ursina to C#/Unity
1. **Entity System**: Converted Ursina's Entity class to Unity MonoBehaviour components
2. **Math Operations**: Migrated Vector3 operations and distance calculations
3. **UI System**: Rebuilt UI using Unity's Canvas system instead of Ursina's direct positioning
4. **Events**: Implemented C# events for component communication
5. **Coroutines**: Used Unity coroutines for animations and timed effects

### Key Differences
- **Performance**: Unity's optimized rendering and physics
- **Input System**: Unity's input manager instead of direct key polling
- **Asset Management**: Unity's asset pipeline for models and textures
- **Scene Management**: Unity's hierarchical scene structure

## Future Development

### Phase 1: Core RPG Features
- [ ] Complete inventory system
- [ ] Equipment with visual changes
- [ ] Basic quest system
- [ ] Save/load functionality

### Phase 2: Advanced Features
- [ ] Multiplayer faction wars
- [ ] Advanced AI behaviors
- [ ] Skill trees and abilities
- [ ] Crafting system

### Phase 3: Polish
- [ ] Sound and music
- [ ] Particle effects
- [ ] Advanced animations
- [ ] Performance optimization

## Known Issues
- Stats UI scrolling may need adjustment for different screen sizes
- Enemy pathfinding is basic (no NavMesh integration yet)
- No visual feedback for stat bonuses from equipment

## Contributing
When adding new features, please:
1. Follow the existing code structure and naming conventions
2. Add appropriate events for UI updates
3. Include debug logging for testing
4. Update this README with new features

## Original Python Version
The original game was built with Ursina engine and can be found at:
https://github.com/JorgeBC420/caminos_de_la_fe

This Unity version maintains the core gameplay while leveraging Unity's powerful features for better performance and expandability.
